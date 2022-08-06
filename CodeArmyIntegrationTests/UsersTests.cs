using DataContracts.Users;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TestUtilities;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class UsersTests
    {
        private readonly GlobalMapper? _mapper;
        private readonly TestServer testServer;
        private readonly HttpClient adminClient;

        public UsersTests(IntegrationTestFixture fixture)
        {
            _mapper = fixture.GlobalMapper;
            testServer = fixture.Server;
            adminClient = fixture.AdminTestClient;
        }

        [Fact]
        public async Task PostUpdateGetDeleteUserAdmin()
        {
            var addUserRequest = TestsUtilities.CreateRandomUserAddRequest();
            await adminClient.AddUserAsync(addUserRequest);

            addUserRequest.LastName = "UpdatedLastName";
            await adminClient.UpdateUserAsync(addUserRequest.Email, addUserRequest);

            var fetchedUser = await adminClient.GetUserAsync(addUserRequest.Email);
            Assert.Equal(_mapper?.ToGetUserResponse(addUserRequest), fetchedUser);

            await adminClient.DeleteUserAsync(addUserRequest.Email);

            var ex = await Assert.ThrowsAsync<CodeArmyException>(() => adminClient.GetUserAsync(addUserRequest.Email));
            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Theory]
        [InlineData(Role.User)]
        [InlineData(Role.Instructor)]
        public async Task GetUpdateCurrentUser(Role role)
        {
            var addUserRequest = TestsUtilities.CreateRandomUserAddRequest(role);
            await adminClient.AddUserAsync(addUserRequest);

            var userClient = testServer.CreateClient(role, addUserRequest.Email);

            addUserRequest.LastName = "UpdatedLastName";
            await userClient.UpdateUserAsync(addUserRequest.Email, addUserRequest);

            var fetchedUser = await userClient.GetCurrentUserAsync();
            Assert.Equal(_mapper?.ToGetUserResponse(addUserRequest), fetchedUser);
        }

        [Theory]
        [InlineData(Role.User)]
        [InlineData(Role.Instructor)]
        public async Task UsersAuthorizationTest(Role role)
        {
            var addUserRequest = TestsUtilities.CreateRandomUserAddRequest();
            var userClient = testServer.CreateClient(role, "nonAdminUser@codearmy.com");

            var ex = await Assert.ThrowsAsync<CodeArmyException>(() => userClient.AddUserAsync(addUserRequest));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => userClient.GetUserAsync(addUserRequest.Email));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => userClient.DeleteUserAsync(addUserRequest.Email));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => userClient.UpdateUserAsync(addUserRequest.Email, addUserRequest));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);
        }
    }
}
