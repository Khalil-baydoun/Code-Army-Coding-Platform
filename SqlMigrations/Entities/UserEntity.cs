using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlMigrations.Entities
{
    [Table(TableName)]
    public class UserEntity
    {
        public const string TableName = "Users";

        [Key, MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string FirstName { get; set; }

        [MaxLength(20)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        public string Salt { get; set; }

        public int Role { get; set; }

        public virtual ICollection<CourseUserEntity> CourseUser { get; set; }
    }
}