using System.Data;
using System.Linq;
using DataContracts.ProblemSets;
using System;
using SqlMigrations.Entities;
using webapi.Store.Interfaces;
using SqlMigrations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Exceptions;
using System.Reflection;
using System.Collections.Generic;
using DataContracts.DueDates;

namespace WebApi.Store.Sql
{
    public class SqlProblemSetStore : IProblemSetStore
    {
        private readonly GlobalMapper _mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private readonly IServiceScopeFactory scopeFactory;

        public SqlProblemSetStore(GlobalMapper _mapper, IServiceScopeFactory scopeFactory)
        {
            this._mapper = _mapper;
            this.scopeFactory = scopeFactory;
        }

        public ProblemSet GetProblemSet(string problemSetId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemSetEntity = db.ProblemSets
                    .Where(ps => ps.Id == Int32.Parse(problemSetId))
                    .Include(ps => ps.Problems)
                    .ToList().FirstOrDefault();
                if (problemSetEntity == null)
                {
                    throw new NotFoundException($"Problem Set with id {problemSetId} was not found");
                }
                return _mapper.ToProblemSet(problemSetEntity);
            }
        }

        public async Task<String> AddProblemSet(AddProblemSetRequest problemSet)
        {
            using (var scope = scopeFactory.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToProblemSetEntity(problemSet);
                    db.ProblemSets.Add(entity);
                    await db.SaveChangesAsync();
                    if (problemSet.ProblemIds != null)
                    {
                        foreach (var probId in problemSet.ProblemIds)
                        {
                            var problem = db.Problems.Where(ps => ps.Id == Int32.Parse(probId)).FirstOrDefault();
                            if (problem != null)
                            {
                                problem.ProblemSetId = entity.Id;
                            }
                            await db.SaveChangesAsync();
                        }

                    }
                    transaction.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }

            }
        }

        public async Task UpdateProblemSet(ProblemSet problemSet)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToProblemSetEntity(problemSet);
                var target = db.ProblemSets
                    .Where(ps => ps.Id == problemSet.Id)
                    .Include(ps => ps.DueDates)
                    .ToList().FirstOrDefault();
                if (target != null)
                {
                    PropertyInfo[] destinationProperties = target.GetType().GetProperties();
                    foreach (PropertyInfo destinationPi in destinationProperties)
                    {
                        PropertyInfo sourcePi = entity.GetType().GetProperty(destinationPi.Name);
                        if (sourcePi.GetValue(entity, null) != null)
                        {
                            destinationPi.SetValue(target, sourcePi.GetValue(entity, null), null);
                        }
                    }
                    var success = await db.SaveChangesAsync() > 0;
                    if (!success)
                    {
                        throw new Exception("Could not update problem set");
                    }
                }
                else
                {
                    throw new NotFoundException($"Problem set with id {problemSet.Id} was not found");
                }
            }
        }

        public async Task AddProblemToProblemSet(int problemSetId, int problemId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problem = db.Problems.Where(ps => ps.Id == problemId).FirstOrDefault();
                if (problem != null)
                {
                    problem.ProblemSetId = problemSetId;
                }
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could add problem to problem set");
                }
            }
        }

        public bool IsOwner(string problemSetId, string userEmail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemSetEntities = db.ProblemSets.Where(ps => Int32.Parse(problemSetId) == ps.Id && userEmail.Equals(ps.AuthorEmail)).ToList();
                return problemSetEntities.Count > 0;
            }
        }

        public async Task DeleteProblemSet(string problemSetId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemSetEntity = db.ProblemSets
                    .First(ps => ps.Id == Int32.Parse(problemSetId));
                db.ProblemSets.Remove(problemSetEntity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete problem set");
                }
            }
        }

        public List<DueDate> GetProblemSetDueDates(string problemSetId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var dueDates = db.DueDates
                    .Where(d => d.problemSetId == Int32.Parse(problemSetId))
                    .Select(d => _mapper.ToDueDate(d))
                    .ToList();
                if (dueDates == null)
                {
                    throw new NotFoundException($"Problem Set with id {problemSetId} has no due dates");
                }
                return dueDates;
            }
        }
    }
}