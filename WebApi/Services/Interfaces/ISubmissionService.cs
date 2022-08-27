using DataContracts.Statistics;
using DataContracts.Submissions;

namespace WebApi.Services.Interfaces
{
    public interface ISubmissionService
    {
        GetSubmissionsResponse GetUserSubmissions(string userEmail, int offset, int limit);

        Task UpdateSubmission(Submission sub);

        Task<Submission?> GetSubmission(int submissionId);

        Task AddSubmission(Submission sub);
    }
}
