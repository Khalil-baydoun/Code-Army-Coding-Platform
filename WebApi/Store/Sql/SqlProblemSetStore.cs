using System.Data;
using DataContracts.ProblemSets;
using webapi.Store.Interfaces;
using SqlMigrations;
using Microsoft.EntityFrameworkCore;
using WebApi.Exceptions;
using SqlMigrations.Entities;
using Utilities;

namespace WebApi.Store.Sql
{
    public class SqlProblemSetStore : IProblemSetStore
    {
        private readonly GlobalMapper _mapper;
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
                    .Where(ps => ps.Id == int.Parse(problemSetId))
                    .Include(ps => ps.ProblemSetProblems)
                    .ThenInclude(ps => ps.Problem)
                    .ToList().FirstOrDefault();

                if (problemSetEntity == null)
                {
                    throw new NotFoundException($"Problem Set with id {problemSetId} was not found");
                }

                return _mapper.ToProblemSet(problemSetEntity);
            }
        }

        public async Task<string> AddProblemSet(AddProblemSetRequest problemSet)
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
                        await db.ProblemSetProblems
                            .AddRangeAsync(problemSet.ProblemIds
                                .Select(problemId => new ProblemSetProblemEntity { ProblemId = int.Parse(problemId), ProblemSetId = entity.Id })
                                .ToArray());
                        await db.SaveChangesAsync();
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
                    .ToList().FirstOrDefault();
                if (target != null)
                {
                    entity.CopyProperties(target);
                    target.DueDate = problemSet.DueDate;
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
                await db.ProblemSetProblems.AddAsync(new ProblemSetProblemEntity { ProblemId = problemId, ProblemSetId = problemSetId });
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not add problem to problem set");
                }
            }
        }

        public async Task<bool> IsOwner(string problemSetId, string userEmail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return await db.ProblemSets.Where(ps => int.Parse(problemSetId) == ps.Id && userEmail.Equals(ps.AuthorEmail)).AnyAsync();
            }
        }

        public async Task DeleteProblemSet(string problemSetId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemSetEntity = db.ProblemSets
                    .First(ps => ps.Id == int.Parse(problemSetId));
                db.ProblemSets.Remove(problemSetEntity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete problem set");
                }
            }
        }
    }
}