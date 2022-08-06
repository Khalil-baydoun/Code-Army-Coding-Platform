using DataContracts.Users;

namespace TestUtilities
{
    public static class UsersTestClient
    {
        private static readonly string usersUrl = "user";

        public static async Task AddUserAsync(this HttpClient client, AddUserRequest addUserRequest)
        {
            await client.SendRequestAsync(HttpMethod.Post, usersUrl, TestsUtilities.ToStringContent(addUserRequest));
        }

        public static async Task DeleteUserAsync(this HttpClient client, string userEmail)
        {
            await client.SendRequestAsync(HttpMethod.Delete, usersUrl + $"/{userEmail}");
        }

        public static async Task<GetUserResponse?> GetUserAsync(this HttpClient client, string userEmail)
        {
            var response = await client.SendRequestAsync(HttpMethod.Get, usersUrl + $"/{userEmail}");
            return await TestsUtilities.ExtractResponse<GetUserResponse>(response);
        }

        public static async Task<GetUserResponse?> GetCurrentUserAsync(this HttpClient client)
        {
            var response = await client.SendRequestAsync(HttpMethod.Get, usersUrl);
            return await TestsUtilities.ExtractResponse<GetUserResponse>(response);
        }

        public static async Task UpdateUserAsync(this HttpClient client, string userEmail, AddUserRequest addUserRequest)
        {
            await client.SendRequestAsync(HttpMethod.Put, usersUrl + $"/{userEmail}", TestsUtilities.ToStringContent(addUserRequest));
        }
    }
}
