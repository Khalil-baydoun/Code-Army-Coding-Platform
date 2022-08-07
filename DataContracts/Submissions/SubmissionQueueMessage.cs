using DataContracts.Statistics;

namespace DataContracts.Submissions
{
    public class SubmissionQueueMessage
    {
        public SubmissionRequest Request { get; set; }

        public SubmissionStatistics SubmissionStatistics { get; set; }
    }
}
