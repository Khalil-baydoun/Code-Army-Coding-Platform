using SqlMigrations;
using WebApi.Store.Interfaces;
using DataContracts.Submissions;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Store.Sql
{
    public class SqlSubmissionssStore : ISubmissionsStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory scopeFactory;

        public SqlSubmissionssStore(GlobalMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _mapper = mapper;
            this.scopeFactory = scopeFactory;
        }

        public async Task AddSubmission(Submission sub)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToSubmissionStatisticsEntity(sub);
                db.Submissions.Add(entity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not add submission");
                }
                sub.Id = entity.Id;
            }
        }

        public async Task UpdateSubmission(Submission sub)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = db.Submissions.Where(ps => ps.Id == sub.Id).FirstOrDefault();
                entity.Verdict = sub.Verdict;
                entity.IsRetried = sub.IsRetried;
                entity.RuntimeErrorMessage = sub.RuntimeErrorMessage;
                entity.CompilerErrorMessage = sub.CompilerErrorMessage;
                entity.TestsPassed = sub.TestsPassed;
                entity.ActualOutput= sub.ActualOutput;
                entity.ExpectedOutput = sub.ExpectedOutput;
                entity.WrongTestInput = sub.WrongTestInput;
                entity.TotalTests = sub.TotalTests;
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not update submission");
                }
            }
        }

        public List<Submission> GetSubmissionsPaged(string userEmail, int offset, int limit)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.Submissions
                    .Where(x => x.UserEmail.Equals(userEmail))
                    .OrderByDescending(x => x.Id)
                    .Skip(offset)
                    .Take(limit)
                    .Select(x => _mapper.ToSubmissionStatistics(x))
                    .ToList();
            }
        }

        public async Task<Submission?> GetSubmission(int submissionId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return await db.Submissions
                    .Where(x => x.Id == submissionId)
                    .Select(x => _mapper.ToSubmissionStatistics(x))
                    .FirstOrDefaultAsync();
            }
        }
    }
}