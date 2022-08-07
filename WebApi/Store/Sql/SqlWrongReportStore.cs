using DataContracts.Report;
using SqlMigrations;
using WebApi.Exceptions;
using WebApi.Store.Interfaces;

namespace WebApi.Store.Sql
{
    public class SqlWrongReportStore : IWaReportStore
    {
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SqlWrongReportStore(GlobalMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void AddReport(WrongAnswerReport wrongAnswerReport)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToWaReportEntity(wrongAnswerReport);
                    db.WaReports.Add(entity);
                    db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }
        }

        public WrongAnswerReport GetWaReport(string submissionId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    var report = db.WaReports
                        .Where(x => x.SubmissionStatisticsId == int.Parse(submissionId))
                        .FirstOrDefault();

                    if (report == null)
                    {
                        throw new NotFoundException("WaReport was not found");
                    }

                    return _mapper.ToWaReport(report);
                }
                catch (Exception e)
                {
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }

            }
        }
    }
}