using DataContracts.Statistics;
using DataContracts.Submissions;

namespace WebApi.Store.Interfaces
{
    public interface IStatisticsService
    {
        UserStatistics GetUserStatistics(string userEmail);

        CourseStatistics GetCourseStatistics(int courseId);

        ProblemStatistics GetProblemStatistics(int problemId);

        Task AddSubmission(SubmissionStatistics sub);

        GetSubmissionsResponse GetUserSubmissions(string userEmail, int offset, int limit);

        Task UpdateSubmission(SubmissionStatistics sub);
    }
}