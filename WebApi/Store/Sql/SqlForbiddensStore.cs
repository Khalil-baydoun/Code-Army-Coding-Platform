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
using DataContracts.Forbiddens;

namespace WebApi.Store.Sql
{
    public class SqlForbiddensStore : IForbiddensStore
    {
        private readonly GlobalMapper _mapper;

        private readonly IServiceScopeFactory scopeFactory;

        public SqlForbiddensStore(IServiceScopeFactory scopeFactory, GlobalMapper _mapper)
        {
            this.scopeFactory = scopeFactory;
            this._mapper = _mapper;
        }

        public async Task AddForbiddens(string problemId, Forbiddens forbiddnes)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToForbiddensEntity(forbiddnes);
                    db.Forbiddens.Add(entity);
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

        public List<Forbiddens> GetForbiddensOfProblem(string problemId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var forbiddensEntities = db.Forbiddens
                .Where(x => x.ProblemId == Int32.Parse(problemId))
                .ToList();
                
                return forbiddensEntities.Select(entity => _mapper.ToForbiddens(entity)).ToList();
            }
        }
    }
}