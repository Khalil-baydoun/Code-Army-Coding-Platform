using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class ProblemEntity
    {
        public const string TableName = "Problems";

        public UserEntity Author { get; set; }

        public virtual SolutionEntity Solution { get; set; }

        [ForeignKey("Author"), MaxLength(50)]
        public string AuthorEmail { get; set; }

        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        public string GeneralDescription { get; set; }

        public string InputDescription { get; set; }

        public string OutputDescription { get; set; }

        public string SampleInput { get; set; }

        public string SampleOutput { get; set; }

        public string Tags { get; set; }

        public string Hints { get; set; }

        public int Difficulty { get; set; }

        [ForeignKey("ProblemSet")]
        public int ProblemSetId { get; set; }

        public bool IsPublic { get; set; }

        public ProblemSetEntity ProblemSet { get; set; }

        public ICollection<TestEntity> Tests { get; set; }
    }
}