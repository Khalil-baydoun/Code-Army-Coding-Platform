using System.Data;
using DataContracts.Problems;
using WebApi.Store.Interfaces;
using WebApi.Exceptions;
using SqlMigrations;
using Utilities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Store.Sql
{
    public class SqlProblemStore : IProblemStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SqlProblemStore(GlobalMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public Problem GetProblem(string problemId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var problemEntity = db.Problems
                    .Where(ps => ps.Id == int.Parse(problemId))
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
                    .First(ps => ps.Id == int.Parse(problemId));
                db.Problems.Remove(problemEntity);
                var success = await db.SaveChangesAsync() > 0;

                if (!success)
                {
                    throw new Exception("Could not delete problem");
                }
            }
        }

        public List<Problem> GetProblems(string userEmail)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.Problems
                    .Where(entity => entity.AuthorEmail.Equals(userEmail))
                    .Select(entity => _mapper.ToProblem(entity)).ToList();
            }
        }

        public List<Problem> GetPublicProblems()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return db.Problems
                    .Where(entity => entity.IsPublic)
                    .Select(entity => _mapper.ToProblem(entity)).ToList();
            }
        }

        public async Task<string> AddProblem(Problem problem)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToProblemEntity(problem);
                    await db.Problems.AddAsync(entity);
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

        public async Task UpdateProblem(Problem problem)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var entity = _mapper.ToProblemEntity(problem);
                var target = db.Problems
                    .Where(ps => ps.Id == problem.Id)
                    .ToList().FirstOrDefault();

                if (target != null)
                {
                    entity.CopyProperties(target);
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

        public async Task<bool> IsOwner(string problemId, string userEmail)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                return await db.Problems
                    .Where(ps => int.Parse(problemId) == ps.Id && userEmail.Equals(ps.AuthorEmail))
                    .AnyAsync();
            }
        }

        public async Task<bool> CanAccessProblem(string problemId, string userEmail)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                // if problem is public or user is owner of problem, then user can access
                if (await db.Problems
                    .Where(p => p.Id == int.Parse(problemId) && (userEmail.Equals(p.AuthorEmail) || p.IsPublic))
                    .AnyAsync())
                {
                    return true;
                }

                // check if a the user belongs to a course that has a problem set that contains this problem
                return await db.ProblemSetProblems
                    .Where(psp => psp.ProblemId == int.Parse(problemId))
                    .Include(psp => psp.ProblemSet)
                    .ThenInclude(ps => ps.Course)
                    .ThenInclude(c => c.CourseUser)
                    .Select(psp => psp.ProblemSet)
                    .Select(ps => ps.Course)
                    .SelectMany(c => c.CourseUser)
                    .Select(cu => cu.UserEmail)
                    .Where(em => em.Equals(userEmail))
                    .AnyAsync();
            }
        }
    }
}