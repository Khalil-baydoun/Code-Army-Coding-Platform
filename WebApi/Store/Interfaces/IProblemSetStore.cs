using DataContracts.ProblemSets;

namespace webapi.Store.Interfaces
{
    public interface IProblemSetStore
    {
        ProblemSet GetProblemSet(string problemSetId);

        Task<string> AddProblemSet(AddProblemSetRequest problemSet);

        Task AddProblemToProblemSet(int problemSetId, int problemId);

        bool IsOwner(string problemSetId, string userEmail);

        Task UpdateProblemSet(ProblemSet problemSet);

        Task DeleteProblemSet(string problemSetId);
    }
}
