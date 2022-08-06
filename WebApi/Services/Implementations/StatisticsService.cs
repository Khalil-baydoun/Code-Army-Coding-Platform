using DataContracts.Statistics;
using DataContracts.Submissions;
using webapi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        IStatisticsStore _statisticsStore;
        IProblemSetService _problemSetService;
        public StatisticsService(IStatisticsStore statisticsStore, IProblemSetService problemSetService)
        {
            _statisticsStore = statisticsStore;
            _problemSetService = problemSetService;
        }

        public async Task AddSubmission(SubmissionStatistics sub)
        {
            await _statisticsStore.AddSubmission(sub);
        }

        public ProblemStatistics GetProblemStatistics(int problemId, int groupId = -1, int problemSetId = -1)
        {
            DateTime upTo = DateTime.Today;
            upTo.AddDays(1);
            if (groupId != -1)
            {
                //var dueDates = _problemSetService.GetProblemSetDueDates(problemSetId.ToString());
                //var dueDate = dueDates.Where(x => x.groupId == groupId).FirstOrDefault();
                //if (dueDate != null)
                //{
                //    upTo = dueDate.dueDate;
                //}
            }
            return new ProblemStatistics
            {
                ProblemId = problemId,
                NumberOfTimesAttempted = _statisticsStore.GetNumberOfTimesAttempted(problemId, upTo, groupId),
                NumberOfTimesSolved = _statisticsStore.GetNumberOfTimesSolved(problemId, upTo, groupId),
                VerdictCounts = _statisticsStore.GetVerdictsOfProblem(problemId, upTo, groupId).GroupBy(x => x).Select(x => new System.Collections.Generic.KeyValuePair<int, int>(x.Key, x.Count())).ToList()
            };
        }

        public UserStatistics GetUserStatistics(string userEmail)
        {
            return new UserStatistics
            {
                UserEmail = userEmail,
                NumberOfAcceptedSubmissions = _statisticsStore.GetNumberOfAcceptedSubmissions(userEmail),
                NumberOfProblemsAttempted = _statisticsStore.GetNumberOfProblemsAttempted(userEmail),
                NumberOfSolvedProblems = _statisticsStore.GetNumberOfProblemsAccepted(userEmail),
                NumberOfSubmissions = _statisticsStore.GetNumberOfSubmissions(userEmail),
                VerdictCounts = _statisticsStore.GetVerdictsOfUser(userEmail).GroupBy(x => x).Select(x => new System.Collections.Generic.KeyValuePair<int, int>(x.Key, x.Count())).ToList()
            };
        }

        public UserStatistics GetUserStatisticsInCourse(string userEmail, int courseId)
        {
            return new UserStatistics
            {
                UserEmail = userEmail,
                VerdictCounts = _statisticsStore.GetVerdictsOfUser(userEmail, courseId).GroupBy(x => x).Select(x => new System.Collections.Generic.KeyValuePair<int, int>(x.Key, x.Count())).ToList(),
                NumberOfAcceptedSubmissions = _statisticsStore.GetNumberOfAcceptedSubmissions(userEmail, courseId),
                NumberOfProblemsAttempted = _statisticsStore.GetNumberOfProblemsAttempted(userEmail, courseId),
                NumberOfSolvedProblems = _statisticsStore.GetNumberOfProblemsAccepted(userEmail, courseId),
                NumberOfSubmissions = _statisticsStore.GetNumberOfSubmissions(userEmail, courseId)
            };
        }

        public UserProblemSetStatistics GetProblemSolvedOfUserInProblemSet(int problemSetId, string userEmail, int groupId = -1)
        {
            DateTime upTo = DateTime.Today;
            upTo.AddDays(1);
            if (groupId != -1)
            {
                //var dueDates = _problemSetService.GetProblemSetDueDates(problemSetId.ToString());
                //var dueDate = dueDates.Where(x => x.groupId == groupId).FirstOrDefault();
                //if (dueDate != null)
                //{
                //    upTo = dueDate.dueDate;
                //}
            }
            return new UserProblemSetStatistics
            {
                UserEmail = userEmail,
                ProblemIdsSolved = _statisticsStore.GetProblemIdsSolvedInProblemSetByUser(userEmail, problemSetId, upTo, groupId)
            };
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