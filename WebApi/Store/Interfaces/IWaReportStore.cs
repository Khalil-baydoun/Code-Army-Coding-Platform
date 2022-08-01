using System.Threading.Tasks;
using DataContracts.Report;
 
namespace WebApi.Store.Interfaces
{
    public interface IWaReportStore
    {
        int AddReport(WrongAnswerReport wrongAnswerReport);
    }
}