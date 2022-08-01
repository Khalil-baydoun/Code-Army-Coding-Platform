using System.Threading.Tasks;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;
using DataContracts.Tests;
using System.Collections.Generic;
using WebApi.Exceptions;

namespace WebApi.Services.Implementations
{
    public class TestService : ITestService
    {
        ITestStore _testStore;
        public TestService(ITestStore testStore)
        {
            _testStore = testStore;
        }

        public List<TestUnit> GetTestsOfProblem(string problemId)
        {
            if (string.IsNullOrWhiteSpace(problemId))
            {
                throw new BadRequestException("Problem Id is empty or missing");
            }
            return _testStore.GetTestsOfProblem(problemId);
        }

        public async Task AddTest(string problemId, TestUnit test)
        {
            if (!IsValidTest(test))
            {
                throw new BadRequestException("Problem Id is empty or missing");
            }
            await _testStore.AddTest(problemId, test);
        }

        public void AddTestBatch(List<TestUnit> tests)
        {
            foreach (var test in tests)
            {
                if (!IsValidTest(test))
                {
                    throw new BadRequestException("Problem Id is empty or missing");
                }
            }
            _testStore.AddTestBatch(tests);
        }

        public async Task DeleteProblemTests(string problemId)
        {
            await _testStore.DeleteProblemTests(problemId);
        }

        private bool IsValidTest(TestUnit test)
        {
            return !string.IsNullOrEmpty(test?.ProblemId) && test != null;
        }
    }
}