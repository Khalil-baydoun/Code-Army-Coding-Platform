using DataContracts.Users;
using Microsoft.AspNetCore.TestHost;

namespace TestUtilities
{
    public class TestUser
    {
        public readonly Role Role;

        public HttpClient Client { get; private set; }

        public AddUserRequest User { get; private set; }

        public static async Task<TestUser> Create(Role role, TestServer testServer, TestUser adminUser)
        {
            TestUser user = new(role);
            await user.Intitialize(testServer, adminUser);
            return user;
        }

        public static TestUser CreateAdmin(TestServer testServer)
        {
            TestUser user = new(Role.Admin);
            user.Client = testServer.CreateClient(Role.Admin);
            return user;
        }

        private TestUser(Role role)
        {
            Role = role;
        }

        private async Task Intitialize(TestServer testServer, TestUser adminUser)
        {
            User = TestsUtilities.CreateRandomUserAddRequest(Role);
            await adminUser.AddUserAsync(User);
            Client = testServer.CreateClient(Role, Email);
        }

        public string Email => User.Email;
    }
}
