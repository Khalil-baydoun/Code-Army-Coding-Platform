using DataContracts.Users;
using System.Net;
using System.Threading.Tasks;
using TestUtilities;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class UsersTests : CodeArmyTestBase
    {

        public UsersTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task PostUpdateGetDeleteUserAdmin()
        {
            var addUserRequest = TestsUtilities.CreateRandomUserAddRequest();
            await adminUser.AddUserAsync(addUserRequest);

            addUserRequest.LastName = "UpdatedLastName";
            await adminUser.UpdateUserAsync(addUserRequest.Email, addUserRequest);

            var fetchedUser = await adminUser.GetUserAsync(addUserRequest.Email);
            Assert.Equal(_mapper?.ToGetUserResponse(addUserRequest), fetchedUser);

            await adminUser.DeleteUserAsync(addUserRequest.Email);

            var ex = await Assert.ThrowsAsync<CodeArmyException>(() => adminUser.GetUserAsync(addUserRequest.Email));
            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Theory]
        [InlineData(Role.User)]
        [InlineData(Role.Instructor)]
        public async Task GetUpdateCurrentUser(Role role)
        {
            TestUser nonAdminUser = await CreateUser(role);

            nonAdminUser.User.LastName = "UpdatedLastName";
            await nonAdminUser.UpdateUserAsync(nonAdminUser.Email, nonAdminUser.User);

            var fetchedUser = await nonAdminUser.GetCurrentUserAsync();
            Assert.Equal(_mapper?.ToGetUserResponse(nonAdminUser.User), fetchedUser);
        }

        [Theory]
        [InlineData(Role.User)]
        [InlineData(Role.Instructor)]
        public async Task UsersAuthorizationTest(Role role)
        {
            TestUser nonAdminUser = await CreateUser(role);

            AddUserRequest addUserRequest = TestsUtilities.CreateRandomUserAddRequest(Role.User);

            var ex = await Assert.ThrowsAsync<CodeArmyException>(() => nonAdminUser.AddUserAsync(addUserRequest));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => nonAdminUser.GetUserAsync(addUserRequest.Email));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => nonAdminUser.DeleteUserAsync(addUserRequest.Email));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => nonAdminUser.UpdateUserAsync(addUserRequest.Email, addUserRequest));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);
        }
    }
}
