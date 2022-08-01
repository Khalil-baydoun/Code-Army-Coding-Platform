using System.Threading.Tasks;
using DataContracts.Submissions;

namespace webapi.Services.Interfaces
{
    public interface IOnlineJudgeService
    {
        SubmissionResponse JudgeCode(SubmissionRequest submissionRequest, bool isSolution = false);
    }
}