using DataContracts.Statistics;

namespace WebApi.Store.Interfaces
{
    public interface IStatisticsStore
    {
        UserStatistics GetUserStatistics(string userEmail);

        CourseStatistics GetCourseStatistics(int courseId);

        public ProblemStatistics GetProblemStatistics(int problemId);
    }
}