using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class ProblemSetProblemEntity
    {
        public const string TableName = "ProblemSetProblem";

        [Key, Column(Order = 0), ForeignKey("Problem")]
        public int ProblemId { get; set; }

        public ProblemSetEntity ProblemSet { get; set; }

        [Key, Column(Order = 1), ForeignKey("ProblemSet")]
        public int ProblemSetId { get; set; }

        public ProblemEntity Problem { get; set; }
    }
}