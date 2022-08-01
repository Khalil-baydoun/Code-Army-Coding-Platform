using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SqlMigrations;
using WebApi.Store.Interfaces;
using DataContracts.Submissions;
using DataContracts.Statistics;
using System;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Store.Sql
{
    public class SqlStatisticsStore : IStatisticsStore
    {
        private readonly GlobalMapper _mapper;

        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private readonly IServiceScopeFactory scopeFactory;

        public SqlStatisticsStore(GlobalMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _mapper = mapper;
            this.scopeFactory = scopeFactory;
        }

        public async Task AddSubmission(SubmissionStatistics sub)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToSubmissionStatisticsEntity(sub);
                db.SubmissionStatistics.Add(entity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not add submission");
                }
                sub.Id = entity.Id;
            }
        }

        public async Task UpdateSubmission(SubmissionStatistics sub)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = db.SubmissionStatistics.Where(ps => ps.Id == sub.Id).FirstOrDefault();
                entity.MemoryTakenInKiloBytes = sub.MemoryTakenInKiloBytes;
                entity.TimeTakenInMilliseconds = sub.TimeTakenInMilliseconds;
                entity.Verdict = sub.Verdict;
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not update submission");
                }
            }
        }

        public List<SubmissionStatistics> GetSubmissionsPaged(string userEmail, int offset, int limit)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                // long totalSubmissions = db.SubmissionStatistics.Where(x => x.UserEmail.Equals(userEmail)).Count();
                return db.SubmissionStatistics.Where(x => x.UserEmail.Equals(userEmail)).OrderByDescending(x => x.Id).Skip(offset).Take(limit).Select(x => _mapper.ToSubmissionStatistics(x)).ToList();
            }
        }

        public int GetNumberOfAcceptedSubmissions(string userEmail, int courseId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                if (courseId != -1)
                {
                    return db.SubmissionStatistics.Where(ps => ps.Problem.ProblemSet.CourseId == courseId && ps.Verdict == (int)Verdict.Accepted && userEmail.Equals(ps.UserEmail)).Count();
                }
                return db.SubmissionStatistics.Where(ps => ps.Verdict == (int)Verdict.Accepted && userEmail.Equals(ps.UserEmail)).Count();
            }
        }

        public int GetNumberOfProblemsAccepted(string userEmail, int courseId = 1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                if (courseId != -1)
                {
                    return db.SubmissionStatistics.Where(ps => ps.Problem.ProblemSet.CourseId == courseId && ps.Verdict == (int)Verdict.Accepted && userEmail.Equals(ps.UserEmail)).Select(ps => ps.ProblemId).Distinct().Count();
                }
                return db.SubmissionStatistics.Where(ps => ps.Verdict == (int)Verdict.Accepted && userEmail.Equals(ps.UserEmail)).Select(ps => ps.ProblemId).Distinct().Count();
            }
        }

        public int GetNumberOfProblemsAttempted(string userEmail, int courseId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                if (courseId != -1)
                {
                    return db.SubmissionStatistics.Where(ps => ps.Problem.ProblemSet.CourseId == courseId && userEmail.Equals(ps.UserEmail)).Select(ps => ps.ProblemId).Distinct().Count();
                }
                return db.SubmissionStatistics.Where(ps => userEmail.Equals(ps.UserEmail)).Select(ps => ps.ProblemId).Distinct().Count();
            }
        }

        public int GetNumberOfSubmissions(string userEmail, int courseId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                if (courseId != -1)
                {
                    return db.SubmissionStatistics.Where(ps => ps.Problem.ProblemSet.CourseId == courseId && userEmail.Equals(ps.UserEmail)).Count();
                }
                return db.SubmissionStatistics.Where(ps => userEmail.Equals(ps.UserEmail)).Count();
            }
        }

        public int GetNumberOfTimesAttempted(int problemId, DateTime upTo, int groupId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.SubmissionStatistics
                    .Where(ps => 
                    problemId == ps.ProblemId && 
                    ps.SubmittedAt < upTo)
                    .Include(ps => ps.User)
                    .Where(ps => (groupId == -1 || ps.User.GroupId == groupId))
                    .Count();
            }
        }

        public int GetNumberOfTimesSolved(int problemId, DateTime upTo, int groupId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.SubmissionStatistics
                    .Where(ps => 
                    ps.Verdict == (int)Verdict.Accepted && 
                    problemId == ps.ProblemId && 
                    ps.SubmittedAt < upTo)
                    .Include(ps => ps.User)
                    .Where(ps => (groupId == -1 || ps.User.GroupId == groupId))
                    .Select(ps => ps.UserEmail)
                    .Distinct().Count();
            }
        }

        public List<int> GetVerdictsOfProblem(int problemId, DateTime upTo, int groupId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var verdicts = db.SubmissionStatistics
                    .Where(ps =>
                    problemId == ps.ProblemId &&
                    ps.SubmittedAt < upTo)
                    .Include(ps => ps.User)
                    .Where(ps => (groupId == -1 || ps.User.GroupId == groupId))
                    .Select(ps => ps.Verdict).ToList();
                return verdicts;
            }
        }
        public List<int> GetVerdictsOfUser(string userEmail, int courseId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                if (courseId != -1)
                {
                    return db.SubmissionStatistics.Where(ps => ps.Problem.ProblemSet.CourseId == courseId && userEmail.Equals(ps.UserEmail)).Select(ps => ps.Verdict).ToList();
                }
                var verdicts = db.SubmissionStatistics.Where(ps => userEmail.Equals(ps.UserEmail)).Select(ps => ps.Verdict).ToList();
                return verdicts;
            }
        }

        public List<int> GetProblemIdsSolvedInProblemSetByUser(string userEmail, int problemSetId, DateTime upTo, int groupId = -1)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var ids = db.SubmissionStatistics
                    .Where(ps =>
                    ps.Verdict == (int)Verdict.Accepted &&
                    ps.Problem.ProblemSet.Id == problemSetId &&
                    userEmail.Equals(ps.UserEmail) &&
                    ps.SubmittedAt < upTo)
                    .Include(ps => ps.User)
                    .Where(ps => (groupId == -1 || ps.User.GroupId == groupId))
                    .Select(ps => ps.ProblemId)
                    .Distinct().ToList();
                return ids;
            }
        }
    }
}