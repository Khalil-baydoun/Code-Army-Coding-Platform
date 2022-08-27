using DataContracts.Statistics;
using DataContracts.Submissions;
using Webapi.JudgingQueue;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private const int minNumberOfHoursToRetrySubmission = 1;
        IStatisticsStore _statisticsStore;
        ISubmissionQueue _submissionQueue;

        public StatisticsService(IStatisticsStore statisticsStore, ISubmissionQueue submissionQueue)
        {
            _statisticsStore = statisticsStore;
            _submissionQueue = submissionQueue;
        }

        public async Task AddSubmission(SubmissionStatistics sub)
        {
            await _statisticsStore.AddSubmission(sub);
        }

        public async Task<SubmissionStatistics?> GetSubmission(int submissionId)
        {
             return await _statisticsStore.GetSubmission(submissionId);
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
            var submissions = _statisticsStore.GetSubmissionsPaged(userEmail, offset, limit);

            // Requeueu submission that are inqueued for more than an hour
            foreach (var submission in submissions)
            {
                if (submission.Verdict == (int)Verdict.InQueue 
                    && !submission.IsRetried
                    && DateTime.Now.Subtract(submission.SubmittedAt).TotalHours >= minNumberOfHoursToRetrySubmission)
                {
                    var submissionRequest = new SubmissionRequest
                    {
                        IsSolution = false,
                        ProblemId = submission.ProblemId.ToString(),
                        ProgLanguage = submission.ProgrammingLanguage,
                        SourceCode = submission.SourceCode,
                        UserEmail = submission.UserEmail,
                        SubmissionId = submission.Id.ToString()
                    };

                    _submissionQueue.EnqueueSubmission(submissionRequest);
                }
            }

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