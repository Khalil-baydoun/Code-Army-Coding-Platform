using DataContracts.Report;
using SqlMigrations;
using WebApi.Exceptions;
using WebApi.Store.Interfaces;

namespace WebApi.Store.Sql
{
    public class SqlWrongReportStore : IWaReportStore
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly GlobalMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public SqlWrongReportStore(GlobalMapper mapper, ISqlConnectionFactory sqlConnectionFactory, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public int AddReport(WrongAnswerReport wrongAnswerReport) 
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    var entity = _mapper.ToWaReportEntity(wrongAnswerReport); //
                    db.WaReports.Add(entity);
                    db.SaveChangesAsync();
                    transaction.Commit();
                    return entity.Id;
                }
                catch (Exception e)
                {
                    transaction.RollbackAsync();
                    throw new BadRequestException("Check for the validity of the parameters", e);
                }
            }


        }


    }
}