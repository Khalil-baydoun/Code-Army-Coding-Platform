using DataContracts.Statistics;
using DataContracts.Submissions;
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

        public async Task AddSubmission(SubmissionStatistics sub)
        {
            await _statisticsStore.AddSubmission(sub);
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

        public GetSubmissionsResponse GetUserSubmissions(string userEmail, int offset, int limit)
        {
            return new GetSubmissionsResponse
            {
                Submissions = _statisticsStore.GetSubmissionsPaged(userEmail, offset, limit),
            };
        }

        public async Task UpdateSubmission(SubmissionStatistics sub)
        {
            await _statisticsStore.UpdateSubmission(sub);
        }
    }
}