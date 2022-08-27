using System;

namespace DataContracts.Statistics
{
    public class SubmissionStatistics
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public int ProblemId { get; set; }

        public int Verdict { get; set; }

        public string SourceCode { get; set; }

        public DateTime SubmittedAt { get; set; }

        public string ProgrammingLanguage { get; set; }

        public bool IsRetried { get; set; }
    }
}