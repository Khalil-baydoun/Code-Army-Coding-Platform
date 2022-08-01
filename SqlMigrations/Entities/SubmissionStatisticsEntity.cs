using System;
using System.Collections.Generic;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class SubmissionStatisticsEntity
    {
        public const string TableName = "SubmissionStatistics";

        public int Id { get; set; }

        public string UserEmail { get; set; }

        public UserEntity User { get; set; }

        public int ProblemId { get; set; }

        public ProblemEntity Problem { get; set; }

        public int Verdict { get; set; }

        public long TimeTakenInMilliseconds { get; set; }

        public long MemoryTakenInKiloBytes { get; set; }

        public string sourceCode { get; set; }

        public DateTime SubmittedAt { get; set; }

        public int ProgrammingLanguage { get; set; }

        public ReportEntity Report {get; set;}
    }
}