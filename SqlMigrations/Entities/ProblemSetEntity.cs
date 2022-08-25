using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class ProblemSetEntity
    {
        public const string TableName = "ProblemSets";

        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Prerequisites { get; set; }

        public UserEntity Author { get; set; }

        [ForeignKey("Author"), MaxLength(50)]
        public string AuthorEmail { get; set; }

        public ICollection<ProblemSetProblemEntity> ProblemSetProblems { get; set; }

        public DateTime? DueDate { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public CourseEntity Course { get; set; }
    }
}