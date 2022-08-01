using System;

namespace DataContracts.Statistics
{
    public class SubmissionStatistics
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public int ProblemId { get; set; }

        public int Verdict { get; set; }

        public long TimeTakenInMilliseconds { get; set; }

        public long MemoryTakenInKiloBytes { get; set; }

        public string SourceCode { get; set; }

        public DateTime SubmittedAt { get; set; }

        public int ProgrammingLanguage { get; set; }
    }
}