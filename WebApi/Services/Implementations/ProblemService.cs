using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Problems;
using WebApi.Exceptions;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class ProblemService : IProblemService
    {
        IProblemStore _problemStore;
        public ProblemService(IProblemStore problemStore)
        {
            _problemStore = problemStore;
        }

        public async Task<string> AddProblem(Problem problem)
        {
            IsValidProblem(problem);
            return await _problemStore.AddProblem(problem);
        }

        private void IsValidProblem(Problem problem)
        {
            if (!(IsValidString(problem.GeneralDescription)
                 && IsValidString(problem.OutputDescription)
                 && IsValidString(problem.Title)
                 && IsValidString(problem.SampleInput)
                 && IsValidString(problem.SampleOutput)))
            {
                throw new BadRequestException("One of the required fields is empty");
            }

            if (problem.GeneralDescription.Length < 20)
            {
                throw new BadRequestException("Description length should be greater than 20 characters");
            }
        }

        public async Task UpdateProblem(Problem problem)
        {
            IsValidProblem(problem);
            await _problemStore.UpdateProblem(problem);
        }

        public Problem GetProblem(string problemId)
        {
            return _problemStore.GetProblem(problemId);
        }

        public async Task DeleteProblem(string problemId)
        {
            await _problemStore.DeleteProblem(problemId);
        }

        public List<Problem> GetProblems()
        {
            return _problemStore.GetProblems();
        }

        public static bool IsValidString(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public bool IsOwner(string problemId, string userEmail)
        {
            return _problemStore.IsOwner(problemId, userEmail);
        }

        public string GetCourseIdOfProblem(string problemId)
        {
            return _problemStore.GetCourseIdOfProblem(problemId);
        }
    }
}