using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class SubmissionStatisticsEntity
    {
        public const string TableName = "SubmissionStatistics";

        [Key]
        public int Id { get; set; }

        [ForeignKey("User"), MaxLength(50)]
        public string UserEmail { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey("Problem")]
        public int ProblemId { get; set; }

        public ProblemEntity Problem { get; set; }

        public int Verdict { get; set; }

        public string SourceCode { get; set; }

        public DateTime SubmittedAt { get; set; }

        public string ProgrammingLanguage { get; set; }

        public bool IsRetried { get; set; }
    }
}