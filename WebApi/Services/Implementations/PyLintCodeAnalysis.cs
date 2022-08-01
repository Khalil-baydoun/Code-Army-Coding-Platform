using System;
using System.IO;
using DataContracts.Submissions;
using DataContracts.Report;
using webapi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class PyLintCodeAnalysis : IOnlineJudgeService
    {

        public readonly IOnlineJudgeService _judgingService;
        IWaReportStore _waReportStore;

        public PyLintCodeAnalysis(IOnlineJudgeService judgingService, IWaReportStore waReportStore)
        {
            _judgingService = judgingService;
            _waReportStore = waReportStore;
        }

        public SubmissionResponse JudgeCode(SubmissionRequest submissionRequest, bool isSolution = false)
        {
            var response = _judgingService.JudgeCode(submissionRequest, isSolution);
            var report = new Report();
            // add wa report if exists (langauge independent)

            if (submissionRequest.ProgLanguage == ProgrammingLanguage.Python)
            {
                string directoryPath = "./submissions/";
                DirectoryInfo di = Directory.CreateDirectory(directoryPath);
                string filePath = $"{directoryPath}/main.py";
                var output = GeneralProcessFileManager.RunProcess("python3", $" -m pylint {filePath}  --rcfile ./Properties/PylintSettings.txt --output-format=text  --score=n", 10000);
                // foreach (var line in output.Output)
                // {
                //     Console.WriteLine(line);
                // }

                report.StaticCodeAnalysis = output.Output.ToArray();
            }

            response.Report = report;
            return response;
        }
    }
}