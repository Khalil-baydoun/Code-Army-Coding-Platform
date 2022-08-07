using System.Collections.Generic;

namespace DataContracts.Statistics
{
    public class ProblemSetStatistics
    {
        public int ProblemSetId { get; set; }
        public List<UserProblemSetStatistics> UserStatistics { get; set; }
    }
}