using DataContracts.Problems;

namespace WebApi.Services.Interfaces
{
    public interface IProblemService
    {
        Problem GetProblem(string problemId);

        Task<string> AddProblem(Problem problem);

        List<Problem> GetProblems(string userEmail);

        List<Problem> GetPublicProblems();

        Task UpdateProblem(Problem problem);

        Task<bool> IsOwner(string problemId, string userEmail);

        Task<bool> CanAccessProblem(string problemId, string userEmail);

        Task DeleteProblem(string problemId);
    }
}