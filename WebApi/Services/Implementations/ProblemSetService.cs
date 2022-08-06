using DataContracts.ProblemSets;
using webapi.Services.Interfaces;
using webapi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class ProblemSetService : IProblemSetService
    {
        IProblemSetStore _problemSetStore;
        public ProblemSetService(IProblemSetStore problemSetStore)
        {
            _problemSetStore = problemSetStore;
        }

        public ProblemSet GetProblemSet(string problemSetId)
        {
            return _problemSetStore.GetProblemSet(problemSetId);
        }

        public async Task<string> AddProblemSet(AddProblemSetRequest problemSet)
        {
            return await _problemSetStore.AddProblemSet(problemSet);
        }

        public async Task AddProblemToProblemSet(int problemSetId, int problemId)
        {
            await _problemSetStore.AddProblemToProblemSet(problemSetId, problemId);
        }

        public bool IsOwner(string problemSetId, string userEmail)
        {
            return _problemSetStore.IsOwner(problemSetId, userEmail);
        }

        public async Task UpdateProblemSet(ProblemSet problemSet)
        {
            await _problemSetStore.UpdateProblemSet(problemSet);
        }

        public async Task DeleteProblemSet(string problemSetId)
        {
            await _problemSetStore.DeleteProblemSet(problemSetId);
        }
    }
}