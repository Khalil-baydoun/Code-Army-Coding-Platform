using WebApi.Store.Interfaces;
using System.Data;
using DataContracts.Tests;
using WebApi.Exceptions;
using SqlMigrations;

namespace WebApi.Store.Sql
{
    public class SqlTestStore : ITestStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory scopeFactory;

        public SqlTestStore(IServiceScopeFactory scopeFactory, GlobalMapper _mapper)
        {
            this.scopeFactory = scopeFactory;
            this._mapper = _mapper;
        }

        public List<TestUnit> GetTestsOfProblem(string problemId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.Tests
                    .Where(x => x.ProblemId == int.Parse(problemId))
                    .Select(entity => _mapper.ToTest(entity))
                    .ToList();
            }
        }

        public async Task AddTest(string problemId, TestUnit test)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToTestEntity(test);
                await db.Tests.AddAsync(entity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not add test");
                }
            }
        }

        public async Task AddTestBatch(List<TestUnit> tests)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    if (tests != null && tests.Count > 0)
                    {
                        await db.Tests.AddRangeAsync(tests.Select(test => _mapper.ToTestEntity(test)).ToArray());
                    }

                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }
        }

        public async Task DeleteProblemTests(string problemId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var testEntities = db.Tests.Where(x => x.ProblemId == int.Parse(problemId));
                db.Tests.RemoveRange(testEntities);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete tests");
                }
            }
        }
    }
}