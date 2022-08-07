using DataContracts.Users;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestUtilities;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class ProblemsTests : CodeArmyTestBase
    {
        public ProblemsTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task PostUpdateGetDeleteProblemInstructor()
        {
            //TestUser instructor = await CreateUser(Role.Instructor);
            //var problem1 = TestsUtilities.CreateRandomAddProblemRequest(instructor.Email);
            //var problem2 = TestsUtilities.CreateRandomAddProblemRequest(instructor.Email);
        }

    }
}
