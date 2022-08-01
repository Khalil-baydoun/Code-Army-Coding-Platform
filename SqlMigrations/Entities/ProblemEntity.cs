using System.Collections.Generic;

namespace SqlMigrations.Entities
{
    [System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
    public class ProblemEntity
    {
        public const string TableName = "Problem";

        public UserEntity Author { get; set; }

        public virtual SolutionEntity Solution { get; set; }

        public string AuthorEmail { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public string GeneralDescription { get; set; }

        public string IDescription { get; set; }

        public string ODescription { get; set; }

        public string SampleInput { get; set; }

        public string SampleOutput { get; set; }

        public int TimeLimitInMilliseconds { get; set; }

        public int TimeFactor { get; set; }
        public int MemoryFactor { get; set; }

        public int MemoryLimitInKiloBytes { get; set; }

        public string Tags { get; set; }

        public string Hints { get; set; }

        public int Difficulty { get; set; }

        public int ProblemSetId { get; set; }

        public ICollection<CommentEntity> Comments { get; set; }
        public bool IsPublic { get; set; }

        public ProblemSetEntity ProblemSet { get; set; }

        public ICollection<TestEntity> Tests { get; set; }
        
        public ICollection<ForbiddensEntity> Forbiddens { get; set; }
    }
}