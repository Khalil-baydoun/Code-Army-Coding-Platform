using DataContracts.Report;

namespace WebApi.Services.Interfaces
{
    public interface IWaReportService
    {
        WrongAnswerReport GetReport(string submissionId);

        void AddReport(WrongAnswerReport report);
    }
}
