using System.Collections.Generic;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class CourseUserEntity
    {
        public const string TableName = "CourseUser";

        public string UserEmail { get; set; }

        public UserEntity User { get; set; }

        public int CourseId { get; set; }

        public CourseEntity Course { get; set; }
    }
}