using System.Threading.Tasks;
using DataContracts.Submissions;

namespace WebApi.Services.Interfaces
{
    public interface ISolutionService
    {
        Task AddSolution(SubmissionRequest submissionRequest);
        Task<string> GetSolution(string problemId, string progLang);
    }
}