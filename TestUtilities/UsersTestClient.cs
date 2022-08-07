using DataContracts.Users;

namespace TestUtilities
{
    public static class UsersTestClient
    {
        private static readonly string usersUrl = "user";


        public static async Task AddUserAsync(this TestUser user, AddUserRequest addUserRequest)
        {
            await user.SendRequestAsync(HttpMethod.Post, usersUrl, TestsUtilities.ToStringContent(addUserRequest));
        }

        public static async Task DeleteUserAsync(this TestUser user, string userEmail)
        {
            await user.SendRequestAsync(HttpMethod.Delete, usersUrl + $"/{userEmail}");
        }

        public static async Task<GetUserResponse?> GetUserAsync(this TestUser user, string userEmail)
        {
            var response = await user.SendRequestAsync(HttpMethod.Get, usersUrl + $"/{userEmail}");
            return await TestsUtilities.ExtractResponse<GetUserResponse>(response);
        }

        public static async Task<GetUserResponse?> GetCurrentUserAsync(this TestUser user)
        {
            var response = await user.SendRequestAsync(HttpMethod.Get, usersUrl);
            return await TestsUtilities.ExtractResponse<GetUserResponse>(response);
        }

        public static async Task UpdateUserAsync(this TestUser user, string userEmail, AddUserRequest addUserRequest)
        {
            await user.SendRequestAsync(HttpMethod.Put, usersUrl + $"/{userEmail}", TestsUtilities.ToStringContent(addUserRequest));
        }
    }
}
