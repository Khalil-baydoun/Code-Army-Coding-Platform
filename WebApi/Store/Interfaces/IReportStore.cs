using System.Threading.Tasks;
using DataContracts.Report;

namespace WebApi.Store.Interfaces
{
    public interface IReportStore
    {
        Task addReport(Report reportStore);
        Report GetReport(string submissionId);
    }
}