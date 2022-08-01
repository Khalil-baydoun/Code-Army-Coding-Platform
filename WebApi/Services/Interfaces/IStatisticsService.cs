using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Statistics;
using DataContracts.Submissions;

namespace WebApi.Store.Interfaces
{
    public interface IStatisticsService
    {
        UserStatistics GetUserStatistics(string userEmail);
        UserStatistics GetUserStatisticsInCourse(string userEmail, int courseId);
        ProblemStatistics GetProblemStatistics(int problemId, int groupId = -1, int problemSetId = -1);
        UserProblemSetStatistics GetProblemSolvedOfUserInProblemSet(int problemSetId, string userEmail, int groupId = -1);
        Task AddSubmission(SubmissionStatistics sub);
        GetSubmissionsResponse GetUserSubmissions(string userEmail, int offset, int limit);
        Task UpdateSubmission(SubmissionStatistics sub);
    }
}