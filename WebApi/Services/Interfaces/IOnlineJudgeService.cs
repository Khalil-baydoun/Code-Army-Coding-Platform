using System.Threading.Tasks;
using DataContracts.Submissions;

namespace webapi.Services.Interfaces
{
    public interface IOnlineJudgeService
    {
        public Task<SubmissionResponse> JudgeCode(SubmissionRequest submissionRequest, bool isSolution = false);
    }
}