using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DataContracts.Report;
using DataContracts.Submissions;
using webapi.Services.Interfaces;
using WebApi.Store.Interfaces;
using DataContracts.Problems;
using DataContracts.Tests;
using System.Threading.Tasks;
using WebApi.Exceptions;
using static WebApi.Services.Implementations.GeneralProcessFileManager;

namespace WebApi.Services.Implementations
{
    public class OnlineJudgeService : IOnlineJudgeService
    {
        private readonly string _submissionsFolderPath = "./submissions/";
        private readonly int _compileTimeLimit = 3000; //3000 milliSeconds
        IProblemStore _problemStore;

        ITestStore _testStore;

        IWaReportStore _waReportStore;



        public OnlineJudgeService(IProblemStore problemStore, ITestStore testStore, IWaReportStore waReportStore)
        {
            _problemStore = problemStore;
            _testStore = testStore;
            _waReportStore = waReportStore;
        }

        private List<ProcessInput> GetProcessInput(Problem problem)
        {
            List<TestUnit> test = _testStore.GetTestsOfProblem(problem.Id.ToString());
            test = test.Where(test => !String.IsNullOrWhiteSpace(test.Input) && !String.IsNullOrWhiteSpace(test.Output)).ToList();
            return new List<ProcessInput>(test.Select(test => new ProcessInput
            {
                TimeLimit = problem.TimeLimitInMilliseconds,
                MemoryLimit = problem.MemoryLimitInKiloBytes,
                Input = test.Input,
                ExpectedOutput = test.Output.Split(new[] { "\r\n" }, StringSplitOptions.None).Where(test => !String.IsNullOrWhiteSpace(test)).ToList()
            })).ToList();
        }

        public SubmissionResponse JudgeCode(SubmissionRequest submissionRequest, bool isSolution = false)
        {
            SubmissionResponse response;
            Problem problem = _problemStore.GetProblem(submissionRequest.ProblemId);
            List<ProcessInput> inp = GetProcessInput(problem);
            DirectoryInfo di = new DirectoryInfo(_submissionsFolderPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            switch (submissionRequest.ProgLanguage)
            {
                case ProgrammingLanguage.Java:
                    {
                        response = RunJavaCode(submissionRequest.SourceCode, inp, isSolution);
                        break;
                    }

                case ProgrammingLanguage.Cpp:
                    {
                        response = RunCppCode(submissionRequest.SourceCode, inp, isSolution);
                        break;
                    }

                case ProgrammingLanguage.Python:
                    {
                        response = RunPythonCode(submissionRequest.SourceCode, inp, isSolution);
                        break;
                    }

                default: throw new BadRequestException("Unrecognized Langauge");
            }
            if (isSolution && response.Verdict == Verdict.Accepted)
            {
                problem.TimeLimitInMilliseconds = problem.TimeFactor * (int)response.TimeTakenInMilliseconds;
                problem.MemoryLimitInKiloBytes = problem.MemoryFactor * (int)response.MemoryTakenInKiloBytes;
                _problemStore.UpdateProblem(problem);
            }
            return response;
        }

        private SubmissionResponse RunJavaCode(string sourceCode, List<ProcessInput> processInput, bool isSolution = false)
        {
            string directoryPath = _submissionsFolderPath;
            string filePath = $"{directoryPath}/main.java";
            GeneralProcessFileManager.CreateFile(filePath, sourceCode, "submissions");
            int exitCode = GeneralProcessFileManager.RunProcess("javac", filePath, _compileTimeLimit).ExitCode;
            if (exitCode != 0)
            {
                return new SubmissionResponse
                {
                    Verdict = Verdict.CompilationError,
                    TimeTakenInMilliseconds = 0
                };
            }
            var response = JudgeAllTests("java", $"-cp {directoryPath} main", processInput, isSolution);
            return response;
        }

        private SubmissionResponse RunCppCode(string sourceCode, List<ProcessInput> processInput, bool isSolution = false)
        {
            string directoryPath = _submissionsFolderPath;
            string filePath = $"{directoryPath}/main.cpp";
            string exectublePath = $"{directoryPath}/mainCpp";
            GeneralProcessFileManager.CreateFile(filePath, sourceCode, "submissions");
            int exitCode = GeneralProcessFileManager.RunProcess("g++", $"{filePath} -o {exectublePath}", _compileTimeLimit).ExitCode;
            if (exitCode != 0)
            {
                return new SubmissionResponse
                {
                    Verdict = Verdict.CompilationError,
                    TimeTakenInMilliseconds = 0
                };
            }

            var response = JudgeAllTests(exectublePath, "", processInput, isSolution);
            return response;
        }

        private SubmissionResponse RunPythonCode(string sourceCode, List<ProcessInput> processInput, bool isSolution = false)
        {
            string directoryPath = _submissionsFolderPath;
            DirectoryInfo di = Directory.CreateDirectory(directoryPath);
            string filePath = $"{directoryPath}/main.py";
            GeneralProcessFileManager.CreateFile(filePath, sourceCode, "submissions");
            var response = JudgeAllTests("python", filePath, processInput, isSolution);
            return response;
        }

        private SubmissionResponse JudgeAllTests(string command, string excutableName, List<ProcessInput> processInput, bool isSolution = false)
        {

            var submissionResponse = new SubmissionResponse
            {
                Verdict = Verdict.Accepted,
                TimeTakenInMilliseconds = 0,
                MemoryTakenInKiloBytes = 0
            };
            int testNumber = 0;
            int solutionTimeLimit = 15000; //15 seconds
            foreach (ProcessInput inp in processInput)
            {
                var processOutput = isSolution ? GeneralProcessFileManager.RunProcess(command, excutableName, solutionTimeLimit, inp.Input) : GeneralProcessFileManager.RunProcess(command, excutableName, inp.TimeLimit, inp.Input);
                var response = JudgeProcessOutput(processOutput, inp, isSolution);
                MergeSubmissionResponses(submissionResponse, response);
                if (response.Verdict != Verdict.Accepted)
                {
                    submissionResponse.Verdict = response.Verdict;
                    if (response.Verdict == Verdict.WrongAnswer)
                    {
                        submissionResponse.WaReport = new WrongAnswerReport
                        {
                            ExpectedOutput = TruncateStringIfExceedsMaxSize(String.Join<string>("\r\n", inp.ExpectedOutput)),
                            Input = TruncateStringIfExceedsMaxSize(inp.Input),
                            ActualOutput = TruncateStringIfExceedsMaxSize(String.Join<string>("\r\n", processOutput.Output))
                        };
                    }
                    break;
                }
                testNumber++;
            }
            submissionResponse.TestsPassed = testNumber;
            return submissionResponse;
        }

        private string TruncateStringIfExceedsMaxSize(string val)
        {
            if (val.Length > 20)
            {
                val = val.Substring(0, 20) + " ...";
            }
            return val;
        }

        private void MergeSubmissionResponses(SubmissionResponse resp1, SubmissionResponse resp2)
        {
            resp1.TimeTakenInMilliseconds = Math.Max(resp1.TimeTakenInMilliseconds, resp2.TimeTakenInMilliseconds);
            resp1.MemoryTakenInKiloBytes = Math.Max(resp1.MemoryTakenInKiloBytes, resp2.MemoryTakenInKiloBytes);
        }

        private SubmissionResponse JudgeProcessOutput(ProcessOutput processOutput, ProcessInput processInput, bool isSolution = false)
        {
            Verdict verdict = Verdict.Accepted;
            Console.WriteLine(processOutput.TimeTaken);
            if (processOutput.TimeTaken >= processInput.TimeLimit && !isSolution)
            {
                verdict = Verdict.TimeLimitExceeded;
            }

            else if (processOutput.MemoryTaken >= processInput.MemoryLimit && !isSolution)
            {
                verdict = Verdict.MemoryLimitExceeded;
            }

            else if (processOutput.ExitCode != 0)
            {
                verdict = Verdict.RuntimeError;
            }

            else if (!IsCorrectOutput(processInput.ExpectedOutput, processOutput.Output))
            {
                verdict = Verdict.WrongAnswer;
            }

            return new SubmissionResponse
            {
                Verdict = verdict,
                TimeTakenInMilliseconds = processOutput.TimeTaken,
                MemoryTakenInKiloBytes = processOutput.MemoryTaken
            };
        }

        bool IsCorrectOutput(List<string> expectedOutput, List<string> actualOutput)
        {
            actualOutput.ForEach(s =>
            {
                s = s.Trim();
                s = s.Replace("\r", "");
                s = s.Replace("\n", "");
            });

            expectedOutput.ForEach(s =>
            {
                s = s.Trim();
                s = s.Replace("\r", "");
                s = s.Replace("\n", "");
            });

            var firstNotSecond = expectedOutput.Except(actualOutput).ToList();
            var secondNotFirst = actualOutput.Except(expectedOutput).ToList();
            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }
    }
}