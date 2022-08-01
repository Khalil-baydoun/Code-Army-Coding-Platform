using DataContracts.Report;

namespace WebApi.Services.Interfaces
{
    public interface IReportService
    {
        Report GetReport(string submissionId);
    }
}
