namespace DataContracts.Statistics
{
    public class UserStatistics
    {
        public string UserEmail { get; set; }

        public int NumberOfSubmissions { get; set; }

        public int NumberOfAcceptedSubmissions { get; set; }

        public int NumberOfProblemsAttempted { get; set; }

        public int NumberOfSolvedProblems { get; set; }

        public List<KeyValuePair<int, int>> VerdictCounts { get; set; }
    }
}