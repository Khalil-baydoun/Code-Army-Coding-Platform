using DataContracts.Problems;

namespace TestUtilities
{
    public static class ProblemsTestsClient
    {
        private static readonly string problemsUrl = "problem";

        public static async Task<string?> CreateProblemAsync(this TestUser user, CreateProblemRequest createProblemRequest)
        {
            var response = await user.SendRequestAsync(HttpMethod.Post, problemsUrl, TestsUtilities.ToStringContent(createProblemRequest));
            return await TestsUtilities.ExtractIdFromResponse(response, "ProblemId");
        }

        public static async Task DeleteProblemAsync(this TestUser user, string problemId)
        {
            await user.SendRequestAsync(HttpMethod.Delete, problemsUrl + $"/{problemId}");
        }

        public static async Task<Problem?> UpdateProblemAsync(this TestUser user, string problemId, UpdateProblemRequest updateRequest)
        {
            var response = await user.SendRequestAsync(HttpMethod.Put, problemsUrl + $"/{problemId}", TestsUtilities.ToStringContent(updateRequest));
            return await TestsUtilities.ExtractResponse<Problem>(response);
        }

        public static async Task<List<Problem>?> GetPublicProblemsAsync(this TestUser user)
        {
            var response = await user.SendRequestAsync(HttpMethod.Get, problemsUrl + "/public");
            return await TestsUtilities.ExtractResponse<List<Problem>>(response);
        }

        public static async Task<List<Problem>?> GetOwnedProblemsAsync(this TestUser user)
        {
            var response = await user.SendRequestAsync(HttpMethod.Get, problemsUrl);
            return await TestsUtilities.ExtractResponse<List<Problem>>(response);
        }
    }
}
