using System;
using System.Linq;
using System.Threading.Tasks;
using DataContracts.Submissions;
using Microsoft.Extensions.DependencyInjection;
using SqlMigrations;
using SqlMigrations.Entities;
using WebApi.Exceptions;
using WebApi.Store.Interfaces;

namespace WebApi.Store.Sql
{
    public class SqlSolutionStore : ISolutionStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SqlSolutionStore(GlobalMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public async Task<string> AddSolution(SubmissionRequest submissionRequest)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToSolutionEntity(submissionRequest);
                    db.Solutions.Add(entity);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return entity.ProblemId.ToString();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }
        }

        public async Task<string> GetSolution(string problemId, string ProgLang)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var solutionEntity = db.Solutions
                    .Where(s => s.ProblemId == Int32.Parse(problemId) && s.ProgLanguage == ProgLang)
                    .ToList().FirstOrDefault();

                if (solutionEntity == null)
                {
                    throw new NotFoundException($"Soltuion for problem with id {problemId} was not found");
                }
                return solutionEntity.SourceCode;
            }
        }
    }
}