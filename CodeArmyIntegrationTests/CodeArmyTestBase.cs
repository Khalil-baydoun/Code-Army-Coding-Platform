using DataContracts.Users;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using TestUtilities;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public abstract class CodeArmyTestBase
    {
        protected readonly TestServer testServer;
        protected readonly TestUser adminUser;
        protected readonly GlobalMapper? _mapper;

        public CodeArmyTestBase(IntegrationTestFixture fixture)
        {
            testServer = fixture.Server;
            adminUser = fixture.AdminTestUser;
            _mapper = fixture.GlobalMapper;
        }

        protected async Task<TestUser> CreateUser(Role role)
        {
            return await TestUser.Create(role, testServer, adminUser);
        }
    }
}
