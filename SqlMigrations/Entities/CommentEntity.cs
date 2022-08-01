using System;

namespace SqlMigrations.Entities
{
    public class CommentEntity
    {
        public Guid Id { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AuthorEmail { get; set; }

        public int problemId { get; set; }

        public virtual UserEntity Author { get; set; } // vritual for Lazy Loading

        public virtual ProblemEntity problem { get; set; }
    }
}