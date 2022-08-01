using System.Collections.Generic;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class ProblemSetEntity
    {
        public const string TableName = "ProblemSet";

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Prerequisites { get; set; }

        public UserEntity Author { get; set; }

        public string AuthorEmail { get; set; }

        public ICollection<ProblemEntity> Problems { get; set; }
        public ICollection<DueDateEntity> DueDates { get; set; }

        public int CourseId { get; set; }

        public CourseEntity Course { get; set; }
    }
}