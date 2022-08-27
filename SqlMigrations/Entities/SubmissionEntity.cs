using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class SubmissionEntity
    {
        public const string TableName = "Submissions";

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

        public int TestsPassed { get; set; } = 0;

        public int TotalTests { get; set; }

        public string? ActualOutput { get; set; }

        public string? ExpectedOutput { get; set; }

        public string? WrongTestInput { get; set; }

        public string? RuntimeErrorMessage { get; set; }

        public string? CompilerErrorMessage { get; set; }
    }
}