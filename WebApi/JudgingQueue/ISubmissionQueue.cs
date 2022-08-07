using DataContracts.Submissions;

namespace Webapi.JudgingQueue
{
    public interface ISubmissionQueue
    {
        Task EnqueueSubmission(SubmissionRequest submissionRequest);
    }
}
