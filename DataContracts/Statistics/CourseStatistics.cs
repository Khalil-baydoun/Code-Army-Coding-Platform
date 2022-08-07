namespace DataContracts.Statistics
{
    public class CourseStatistics
    {
        public int CourseId { get; set; }

        public List<UserStatistics> UserStatistics { get; set; }

        public List<ProblemSetStatistics> ProblemSetStatistics { get; set; }
    }
}
