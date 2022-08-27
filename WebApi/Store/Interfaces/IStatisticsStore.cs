using DataContracts.Statistics;

namespace WebApi.Store.Interfaces
{
    public interface IStatisticsStore
    {
        UserStatistics GetUserStatistics(string userEmail);

        CourseStatistics GetCourseStatistics(int courseId);

        public ProblemStatistics GetProblemStatistics(int problemId);

        Task AddSubmission(SubmissionStatistics sub);

        Task UpdateSubmission(SubmissionStatistics sub);

        List<SubmissionStatistics> GetSubmissionsPaged(string userEmail, int offset, int limit);

        Task<SubmissionStatistics?> GetSubmission(int submissionId);
    }
}