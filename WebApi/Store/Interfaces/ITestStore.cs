using DataContracts.Tests;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace WebApi.Store.Interfaces
{
    public interface ITestStore
    {
        Task AddTest(string problemId, TestUnit test);
        List<TestUnit> GetTestsOfProblem(string problemId);
        Task AddTestBatch(List<TestUnit> tests);
        Task DeleteProblemTests(string problemId);
    }
}