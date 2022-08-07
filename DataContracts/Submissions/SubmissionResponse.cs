using DataContracts.Tests;
using DataContracts.Report;

namespace DataContracts.Submissions
{
    public class SubmissionResponse
    {
        public Verdict Verdict { get; set; }

        public int TestsPassed { get; set; }

        public WrongAnswerReport WaReport { get; set; }
    }

    // Values according to ideone API
    public enum Verdict
    {
        Accepted = 15,
        WrongAnswer = 2,
        CompilationError = 11,
        RuntimeError = 12,
        MemoryLimitExceeded = 17,
        TimeLimitExceeded = 13,
        InternalError = 20,
        InQueue = 1,
        ForbiddenKeyword = 0
    }
}