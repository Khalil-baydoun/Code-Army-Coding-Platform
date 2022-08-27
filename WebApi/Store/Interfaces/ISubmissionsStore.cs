using DataContracts.Submissions;

namespace WebApi.Store.Interfaces
{
    public interface ISubmissionsStore
    {
        Task AddSubmission(Submission sub);

        Task UpdateSubmission(Submission sub);

        List<Submission> GetSubmissionsPaged(string userEmail, int offset, int limit);

        Task<Submission?> GetSubmission(int submissionId);
    }
}