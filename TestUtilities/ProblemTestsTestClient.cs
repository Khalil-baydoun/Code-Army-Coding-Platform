using DataContracts.Tests;

namespace TestUtilities
{
    public static class ProblemTestsTestClient
    {
        private static readonly string testsUrl = "test";

        public static async Task<List<TestUnit>?> GetProblemTestsAsync(this TestUser user, string problemId)
        {
            var response = await user.SendRequestAsync(HttpMethod.Get, testsUrl + $"/{problemId}");
            return await TestsUtilities.ExtractResponse<List<TestUnit>>(response);
        }

        public static async Task UploadProblemTestsAsync(this TestUser user, string problemId, string inputFileName, string outputFileName)
        {
            await user.SendRequestAsync(HttpMethod.Post, testsUrl + "/uploadTests", TestsUtilities.CreateTestFileUploadContent(inputFileName, outputFileName, problemId));
        }

        public static async Task UploadSingleProblemTestAsync(this TestUser user, string problemId, string input, string output)
        {
            var test = new TestUnit()
            {
                Input = input,
                Output = output,
                ProblemId = problemId
            };

            await user.SendRequestAsync(HttpMethod.Post, testsUrl, TestsUtilities.ToStringContent(test));
        }

        public static async Task DeleteProblemTestsAsync(this TestUser user, string problemId)
        {
            await user.SendRequestAsync(HttpMethod.Delete, testsUrl + $"/{problemId}");
        }
    }
}
