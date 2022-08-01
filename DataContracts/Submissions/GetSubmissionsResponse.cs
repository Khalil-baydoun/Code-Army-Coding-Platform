using System.Collections.Generic;
using DataContracts.Statistics;

namespace DataContracts.Submissions
{
    public class GetSubmissionsResponse
    {
        public List<SubmissionStatistics> Submissions { get; set; }

        public bool SubmissionsRemaining { get; set; }
    }
}