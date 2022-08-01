using System.Collections.Generic;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class CourseEntity
    {
        public const string TableName = "Course";

        public int Id { get; set; }

        public string Name { get; set; }

        public UserEntity Author { get; set; }

        public string AuthorEmail { get; set; }

        public string Description { get; set; }

        public ICollection<ProblemSetEntity> ProblemSets { get; set; }
        
        public ICollection<CourseUserEntity> CourseUser { get; set; }
    }
}