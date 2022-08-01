using WebApi.Store.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebApi.Store.Sql.Dapper;
using DataContracts.Tests;
using SqlMigrations.Entities;
using Dapper;
using System;
using WebApi.Exceptions;
using Microsoft.Extensions.DependencyInjection;
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
                var testEntities = db.Tests.Where(x => x.ProblemId == Int32.Parse(problemId)).ToList();
                return testEntities.Select(entity => _mapper.ToTest(entity)).ToList();
            }
        }

        public async Task AddTest(string problemId, TestUnit test)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToTestEntity(test);
                db.Tests.Add(entity);
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
                    foreach (var test in tests)
                    {
                        var entity = _mapper.ToTestEntity(test);
                        db.Tests.Add(entity);
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
                var TestEntities = db.Tests.Where(x => x.ProblemId == Int32.Parse(problemId));
                db.Tests.RemoveRange(TestEntities);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete tests");
                }
            }
        }
    }
}