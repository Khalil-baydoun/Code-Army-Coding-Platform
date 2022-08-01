using System;
using System.Linq;
using System.Threading.Tasks;
using DataContracts.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SqlMigrations;
using SqlMigrations.Entities;
using WebApi.Exceptions;
using WebApi.Store.Interfaces;

namespace WebApi.Store.Sql
{
    public class SqlReportStore : IReportStore
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SqlReportStore(GlobalMapper mapper, ISqlConnectionFactory sqlConnectionFactory, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task addReport(Report report)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToReportEntity(report);
                    db.Reports.Add(entity);
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

        public Report GetReport(string submissionId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    var report = db.Reports
                        .Where(x => x.Id == Int32.Parse(submissionId))
                        .Include(x => x.WaReport)
                        .FirstOrDefault();
                    return _mapper.ToReport(report);
                }
                catch (Exception e)
                {
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }

            }
        }
    }
}