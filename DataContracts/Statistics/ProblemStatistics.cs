using System.Collections.Generic;

namespace DataContracts.Statistics
{
    public class ProblemStatistics
    {
        public int ProblemId { get; set; }

        public int NumberOfTimesAttempted { get; set; }

        public int NumberOfTimesSolved { get; set; }

        public List<KeyValuePair<int, int>> VerdictCounts { get; set; }
    }
}