using DataContracts.Statistics;

namespace DataContracts.Submissions
{
    public class GetSubmissionsResponse
    {
        public List<Submission> Submissions { get; set; }

        public bool SubmissionsRemaining { get; set; }
    }
}