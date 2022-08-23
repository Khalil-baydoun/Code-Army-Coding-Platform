using DataContracts;
using DataContracts.Jobe;
using DataContracts.Problems;
using DataContracts.Report;
using DataContracts.Submissions;
using DataContracts.Tests;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using webapi.Services.Interfaces;
using Webapi.Services.Implementations.Settings;
using WebApi.Store.Interfaces;

namespace webapi.Services.Implementations
{
    public class JobeJudgingService : IOnlineJudgeService
    {
        private readonly IProblemStore _problemStore;
        private readonly ITestStore _testStore;
        private static readonly HttpClient client = new();
        private readonly string jobeEndpoint;

        public JobeJudgingService(IProblemStore problemStore, ITestStore testStore, IOptions<JobeServerSettings> options)
        {
            _problemStore = problemStore;
            _testStore = testStore;
            jobeEndpoint = options.Value.Endpoint;
        }

        private async Task<JobeSubmissionResponse> SendRequestToJobeServer(TestUnit testUnit, SubmissionRequest submissionRequest, Problem problem)
        {
            var jobeSubmissionRequest = new
            {
                run_spec = new
                {
                    language_id = submissionRequest.ProgLanguage,
                    sourcecode = submissionRequest.SourceCode,
                    input = testUnit.Input,
                    parameters = new
                    {
                        cputime = problem.TimeLimitInMilliseconds / 1000,
                        memorylimit = problem.MemoryLimitInKiloBytes / 1000
                    }
                }
            };

            var resp = await client.PostAsync(
                 jobeEndpoint,
                 new StringContent(JsonSerializer.Serialize(jobeSubmissionRequest), Encoding.UTF8, "application/json"));

            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception("Jobe Server call was not successfull");
            }

            var contentStream = await resp.Content.ReadAsStreamAsync();
            var parsedResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(contentStream);

            var jobeOutput = new JobeSubmissionResponse
            {
                Outcome = int.Parse(parsedResponse["outcome"].ToString()),
                Output = TrimAndSplitOnNewLine(parsedResponse["stdout"].ToString())
            };

            if (jobeOutput.Outcome == (int)Verdict.Accepted)
            {
                if (!IsCorrectOutput(jobeOutput.Output, TrimAndSplitOnNewLine(testUnit.Output)))
                {
                    jobeOutput.Outcome = (int)Verdict.WrongAnswer;
                }
            }

            return jobeOutput;
        }

        public async Task<SubmissionResponse> JudgeCode(SubmissionRequest submissionRequest, bool isSolution = false)
        {
            Problem problem = _problemStore.GetProblem(submissionRequest.ProblemId);
            List<TestUnit> tests = _testStore.GetTestsOfProblem(problem.Id.ToString());
            return await JudgeAllTests(submissionRequest, tests, problem);
        }

        private async Task<SubmissionResponse> JudgeAllTests(SubmissionRequest req, List<TestUnit> tests, Problem problem)
        {

            var submissionResponse = new SubmissionResponse
            {
                Verdict = Verdict.Accepted,
            };

            int testNumber = 0;
            foreach (var test in tests)
            {
                var jobeOutput = await SendRequestToJobeServer(test, req, problem);

                if (jobeOutput.Outcome != (int)Verdict.Accepted)
                {
                    submissionResponse.Verdict = (Verdict)jobeOutput.Outcome;
                    if (submissionResponse.Verdict == Verdict.WrongAnswer)
                    {
                        submissionResponse.WaReport = new WrongAnswerReport
                        {
                            ExpectedOutput = TruncateStringIfExceedsMaxSize(test.Output),
                            Input = TruncateStringIfExceedsMaxSize(test.Input),
                            ActualOutput = TruncateStringIfExceedsMaxSize(string.Join("\r\n", jobeOutput.Output))
                        };
                    }
                    break;
                }

                testNumber++;
            }

            submissionResponse.TestsPassed = testNumber;
            return submissionResponse;
        }

        private static string TruncateStringIfExceedsMaxSize(string val)
        {
            if (val.Length > 200)
            {
                val = val[..200] + " ...";
            }

            return val;
        }

        private static bool IsCorrectOutput(List<string> expectedOutput, List<string> actualOutput)
        {
            var firstNotSecond = expectedOutput.Except(actualOutput).ToList();
            var secondNotFirst = actualOutput.Except(expectedOutput).ToList();
            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }

        private static List<string> TrimAndSplitOnNewLine(string val)
        {
            return val.Split(new[] { '\r', '\n' })
                .Select(str => str.Trim())
                .Where(str => str.Length > 0)
                .ToList();
        }
    }
}
