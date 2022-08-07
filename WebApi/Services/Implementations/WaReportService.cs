using DataContracts.Report;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class WaReportService : IWaReportService
    {
        private readonly IWaReportStore _waReportStore;

        public WaReportService(IWaReportStore reportStore)
        {
            _waReportStore = reportStore;
        }

        public void AddReport(WrongAnswerReport report)
        {
            _waReportStore.AddReport(report);
        }

        public WrongAnswerReport GetReport(string submissionId)
        {
            return _waReportStore.GetWaReport(submissionId);
        }
    }
}