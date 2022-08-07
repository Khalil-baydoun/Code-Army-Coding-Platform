using DataContracts.Users;
using System.Threading.Tasks;
using TestUtilities;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class ProblemTestUnitsTests : CodeArmyTestBase 
    {
        public ProblemTestUnitsTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task PostGetDeleteProblemTests()
        {
            var instructor = await CreateUser(Role.Instructor);
            var problem = TestsUtilities.CreateRandomAddProblemRequest(instructor.Email);
            var problemId = await instructor.CreateProblemAsync(problem);

            await instructor.UploadProblemTestsAsync(problemId, "SampleInputAdd.txt", "SampleOutputAdd.txt");
            var tests = await instructor.GetProblemTestsAsync(problemId);
            Assert.Equal(2, tests?.Count);

            await instructor.DeleteProblemTestsAsync(problemId);

            await instructor.UploadSingleProblemTestAsync(problemId, "testInput", "testOutput");

            var testUnits = await instructor.GetProblemTestsAsync(problemId);

            Assert.Single(testUnits);
            Assert.Equal("testInput", testUnits[0].Input);
            Assert.Equal("testOutput", testUnits[0].Output);
        }
    }
}
