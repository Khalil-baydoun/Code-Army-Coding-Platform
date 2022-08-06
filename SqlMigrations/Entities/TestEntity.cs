using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class TestEntity
    {
        public const string TableName = "Tests";

        [Key]
        public int Id { get; set; }

        public ProblemEntity Problem { get; set; }

        [ForeignKey("Problem")]
        public int ProblemId { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}