using System.Collections.Generic;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class UserEntity
    {
        public const string TableName = "users";

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public int Role { get; set; }

        public ICollection<CourseUserEntity> CourseUser { get; set; }
        public int GroupId { get; set; }
        public GroupEntity Group { get; set; }
    }
}