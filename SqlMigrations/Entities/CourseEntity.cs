using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class CourseEntity
    {
        public const string TableName = "Courses";

        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public UserEntity Author { get; set; }

        [ForeignKey("Author"), MaxLength(50)]
        public string AuthorEmail { get; set; }
        
        public string Description { get; set; }

        public ICollection<ProblemSetEntity> ProblemSets { get; set; }
        
        public ICollection<CourseUserEntity> CourseUser { get; set; }
    }
}