using DataContracts.Submissions;
using Webapi.JudgingQueue;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class SubmissionsService : ISubmissionService
    {
        private const int minNumberOfHoursToRetrySubmission = 1;
        ISubmissionsStore _submissionsStore;
        ISubmissionQueue _submissionQueue;

        public SubmissionsService(ISubmissionsStore submissionsStore, ISubmissionQueue submissionQueue)
        {
            _submissionsStore = submissionsStore;
            _submissionQueue = submissionQueue;
        }

        public async Task AddSubmission(Submission sub)
        {
            await _submissionsStore.AddSubmission(sub);
        }

        public async Task<Submission?> GetSubmission(int submissionId)
        {
            return await _submissionsStore.GetSubmission(submissionId);
        }

        public GetSubmissionsResponse GetUserSubmissions(string userEmail, int offset, int limit)
        {
            var submissions = _submissionsStore.GetSubmissionsPaged(userEmail, offset, limit);

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
                Submissions = _submissionsStore.GetSubmissionsPaged(userEmail, offset, limit),
            };
        }

        public async Task UpdateSubmission(Submission sub)
        {
            await _submissionsStore.UpdateSubmission(sub);
        }
    }
}