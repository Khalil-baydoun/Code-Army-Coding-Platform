using System.Collections.Generic;

namespace DataContracts.Statistics
{
    public class ProblemSetStatistics
    {
        public List<ProblemStatistics> ProblemsStatistics { get; set; }

        public List<UserProblemSetStatistics> UserStatistics { get; set; }
    }
}