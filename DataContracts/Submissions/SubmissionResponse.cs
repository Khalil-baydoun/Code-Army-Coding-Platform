using DataContracts.Tests;
using DataContracts.Report;

namespace DataContracts.Submissions
{
    public class SubmissionResponse
    {
        public Verdict Verdict { get; set; }

        public long TimeTakenInMilliseconds { get; set; }

        public long MemoryTakenInKiloBytes { get; set; }

        public int TestsPassed { get; set; }

        public WrongAnswerReport WaReport { get; set; }
        public Report.Report Report {get; set;}
    }

    public enum Verdict
    {
        Accepted,
        WrongAnswer,
        CompilationError,
        RuntimeError,
        MemoryLimitExceeded,
        TimeLimitExceeded,
        InQueue,
        ForbiddenKeyword
    }
}