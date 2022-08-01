using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Problems;

namespace WebApi.Services.Interfaces
{
    public interface IProblemService
    {
        Problem GetProblem(string problemId);

        Task<string> AddProblem(Problem problem);

        List<Problem> GetProblems();

        Task UpdateProblem(Problem problem);

        bool IsOwner(string problemId, string userEmail);

        Task DeleteProblem(string problemId);
        string GetCourseIdOfProblem(string problemId);
    }
}