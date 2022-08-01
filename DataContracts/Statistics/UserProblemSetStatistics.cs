using System.Collections.Generic;

namespace DataContracts.Statistics
{
    public class UserProblemSetStatistics
    {
        public string UserEmail { get; set; }

        public List<int> ProblemIdsSolved { get; set; }
    }
}