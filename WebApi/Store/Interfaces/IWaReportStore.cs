using System.Threading.Tasks;
using DataContracts.Report;
 
namespace WebApi.Store.Interfaces
{
    public interface IWaReportStore
    {
        void AddReport(WrongAnswerReport wrongAnswerReport);

        WrongAnswerReport GetWaReport(string submissionId);
    }
}