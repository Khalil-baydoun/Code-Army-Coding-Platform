using System.Threading.Tasks;
using DataContracts.Tests;
using System.Collections.Generic;

namespace WebApi.Services.Interfaces
{
    public interface ITestService
    {
        List<TestUnit> GetTestsOfProblem(string problemId);

        Task AddTest(string problemId, TestUnit test);
        void AddTestBatch(List<TestUnit> tests);
        Task DeleteProblemTests(string problemId);
    }
}