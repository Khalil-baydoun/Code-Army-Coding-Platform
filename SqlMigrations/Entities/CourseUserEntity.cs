using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class CourseUserEntity
    {
        public const string TableName = "CourseUsers";

        [Key, Column(Order = 0), ForeignKey("User"), MaxLength(50)]
        public string UserEmail { get; set; }

        public UserEntity User { get; set; }

        [Key, Column(Order = 1), ForeignKey("Course")]
        public int CourseId { get; set; }

        public CourseEntity Course { get; set; }
    }
}