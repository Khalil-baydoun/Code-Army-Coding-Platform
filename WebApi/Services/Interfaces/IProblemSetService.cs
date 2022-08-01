using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.DueDates;
using DataContracts.ProblemSets;

namespace webapi.Services.Interfaces
{
    public interface IProblemSetService
    {
        ProblemSet GetProblemSet(string problemSetId);
        Task<string> AddProblemSet(AddProblemSetRequest problemSet);
        Task AddProblemToProblemSet(int problemSetId, int problemId);
        bool IsOwner(string problemSetId, string userEmail);
        Task UpdateProblemSet(ProblemSet problemSet);
        Task DeleteProblemSet(string problemSetId);
        List<DueDate> GetProblemSetDueDates(string problemSetId);
    }
}
