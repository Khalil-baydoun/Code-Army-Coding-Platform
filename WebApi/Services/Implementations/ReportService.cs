using DataContracts.Report;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class ReportService : IReportService
    {
        IReportStore _reportStore;
        public ReportService(IReportStore reportStore)
        {
            _reportStore = reportStore;
        }
        public Report GetReport(string submissionId)
        {
            var report = _reportStore.GetReport(submissionId);
            return report;
        }
    }
}