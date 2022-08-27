using DataContracts.Statistics;
using DataContracts.Submissions;
using Webapi.JudgingQueue;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        IStatisticsStore _statisticsStore;

        public StatisticsService(IStatisticsStore statisticsStore)
        {
            _statisticsStore = statisticsStore;
        }

        public CourseStatistics GetCourseStatistics(int courseId)
        {
            return _statisticsStore.GetCourseStatistics(courseId);
        }

        public ProblemStatistics GetProblemStatistics(int problemId)
        {
            return _statisticsStore.GetProblemStatistics(problemId);
        }

        public UserStatistics GetUserStatistics(string userEmail)
        {
            return _statisticsStore.GetUserStatistics(userEmail);
        }
    }
}