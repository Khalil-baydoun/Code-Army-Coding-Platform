using DataContracts.Statistics;

namespace DataContracts.Submissions
{
    public class SubmissionQueueMessage
    {
        public SubmissionRequest Request { get; set; }

        public Submission Submission { get; set; }

        public bool IsyRetry { get; set; } = false;
    }
}
