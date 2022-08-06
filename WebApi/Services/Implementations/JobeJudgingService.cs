using DataContracts;
using DataContracts.Problems;
using DataContracts.Submissions;
using DataContracts.Tests;
using System.Text;
using System.Text.Json;
using webapi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace webapi.Services.Implementations
{
    public class JobeJudgingService : IOnlineJudgeService
    {
        IProblemStore _problemStore;
        ITestStore _testStore;
        IWaReportStore _waReportStore;
        static readonly HttpClient client = new HttpClient();
        
        public JobeJudgingService(IProblemStore problemStore, ITestStore testStore, IWaReportStore waReportStore) // string jobeendpoint)
        {
            _problemStore = problemStore;
            _testStore = testStore;
            _waReportStore = waReportStore;
        }

        private async Task<HttpResponseMessage> SendRequestToJobeServer(TestUnit testUnit, SubmissionRequest submissionRequest)
        {
            var jobeSubmissionRequest = new JobeSubmissionRequest
            {
                run_spec = new RunSpec
                {
                    language_id = submissionRequest.ProgLanguage,
                    sourcecode = submissionRequest.SourceCode,
                    input = testUnit.Input
                    //parameters = new Parameters   TODOO
                    //{

                    //}

                }
            };

            var s = (JsonSerializer.Serialize(jobeSubmissionRequest));
            return await client.PostAsync(
                "http://localhost:4000/jobe/index.php/restapi/runs",
                 new StringContent(JsonSerializer.Serialize(jobeSubmissionRequest), Encoding.UTF8, "application/json"));
        }

        


        public async Task<SubmissionResponse> JudgeCode(SubmissionRequest submissionRequest, bool isSolution = false)
        {
            SubmissionResponse response = null;
            Problem problem = _problemStore.GetProblem(submissionRequest.ProblemId);
            List<TestUnit> tests = _testStore.GetTestsOfProblem(problem.Id.ToString());
            var jobeResponse = await SendRequestToJobeServer(tests[0], submissionRequest);

            return null;
        }
    }
}
