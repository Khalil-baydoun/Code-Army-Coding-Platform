using System.Threading.Tasks;
using DataContracts.Submissions;

namespace WebApi.Store.Interfaces
{
    public interface ISolutionStore
    {
        Task<string> AddSolution(SubmissionRequest submissionRequest);
        Task<string> GetSolution(string problemId, ProgrammingLanguage ProgLang);
    }
}