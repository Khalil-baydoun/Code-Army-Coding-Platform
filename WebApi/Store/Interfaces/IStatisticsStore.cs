using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Statistics;
using DataContracts.Submissions;

namespace WebApi.Store.Interfaces
{
    public interface IStatisticsStore
    {
        int GetNumberOfProblemsAccepted(string userEmail, int courseId = -1);
        int GetNumberOfProblemsAttempted(string userEmail, int courseId = -1);
        int GetNumberOfSubmissions(string userEmail, int courseId = -1);
        int GetNumberOfAcceptedSubmissions(string userEmail, int courseId = -1);
        int GetNumberOfTimesAttempted(int problemId, DateTime upTo, int groupId = -1);
        int GetNumberOfTimesSolved(int problemId, DateTime upTo, int groupId = -1);
        List<int> GetVerdictsOfProblem(int problemId, DateTime upTo, int groupId = -1);
        List<int> GetVerdictsOfUser(string userEmail, int courseId = -1);
        Task AddSubmission(SubmissionStatistics sub);
        List<int> GetProblemIdsSolvedInProblemSetByUser(string userEmail, int problemSetId, DateTime upTo, int groupId = -1);
        List<SubmissionStatistics> GetSubmissionsPaged(string userEmail, int offset, int limit);
        Task UpdateSubmission(SubmissionStatistics sub);
    }
}