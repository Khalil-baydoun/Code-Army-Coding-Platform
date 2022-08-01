using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataContracts.Problems;
using WebApi.Store.Interfaces;
using System;
using WebApi.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using SqlMigrations;
using DataContracts.Comments;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AutoMapper;
using SqlMigrations.Entities;

namespace WebApi.Store.Sql
{
    public class SqlProblemStore : IProblemStore
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SqlProblemStore(GlobalMapper mapper, ISqlConnectionFactory sqlConnectionFactory, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public Problem GetProblem(string problemId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemEntity = db.Problems
                    .Where(ps => ps.Id == Int32.Parse(problemId))
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                    .ToList().FirstOrDefault();

                if (problemEntity == null)
                {
                    throw new NotFoundException($"Problem with id {problemId} was not found");
                }
                return _mapper.ToProblem(problemEntity);
            }
        }

        public async Task DeleteProblem(string problemId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemEntity = db.Problems
                    .First(ps => ps.Id == Int32.Parse(problemId));
                db.Problems.Remove(problemEntity);
                var success = await db.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new Exception("Could not delete problem");
                }
            }
        }

        public List<Problem> GetProblems()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemEntities = db.Problems
                    .ToList();
                return problemEntities.Select(entity => _mapper.ToProblem(entity)).ToList();
            }
        }

        public async Task<String> AddProblem(Problem problem)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToProblemEntity(problem);
                    db.Problems.Add(entity);
                    await db.SaveChangesAsync();
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

        public string GetCourseIdOfProblem(string problemId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var courseId = db.Problems.Where(x => x.Id == Int32.Parse(problemId)).Select(x => x.ProblemSet.CourseId).FirstOrDefault();
                return courseId.ToString();
            }
        }
        public async Task UpdateProblem(Problem problem)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToProblemEntity(problem);
                var target = db.Problems
                    .Where(ps => ps.Id == problem.Id)
                    .ToList().FirstOrDefault();
                entity.ProblemSetId = target.ProblemSetId;
                if (target != null)
                {
                    PropertyInfo[] destinationProperties = target.GetType().GetProperties();
                    foreach (PropertyInfo destinationPi in destinationProperties)
                    {
                        PropertyInfo sourcePi = entity.GetType().GetProperty(destinationPi.Name);
                        destinationPi.SetValue(target, sourcePi.GetValue(entity, null), null);
                    }
                    var success = await db.SaveChangesAsync() > 0;
                    if (!success)
                    {
                        throw new Exception("Could not update problem");
                    }
                }
                else
                {
                    throw new NotFoundException($"Problem with id {problem.Id} was not found");
                }
            }
        }

        public bool IsOwner(string problemId, string userEmail)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemEntities = db.Problems.Where(ps => Int32.Parse(problemId) == ps.Id && userEmail.Equals(ps.AuthorEmail)).ToList();
                return problemEntities.Count > 0;
            }
        }
    }
}