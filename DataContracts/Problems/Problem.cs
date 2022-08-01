using System.Collections.Generic;
using DataContracts.Comments;
using DataContracts.Submissions;
using DataContracts.Tests;

namespace DataContracts.Problems
{
    public class Problem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string AuthorEmail { get; set; }

        public string GeneralDescription { get; set; }

        public string IDescription { get; set; }

        public string ODescription { get; set; }

        public string SampleInput { get; set; }

        public string SampleOutput { get; set; }

        public int TimeLimitInMilliseconds { get; set; }
        public int TimeFactor { get; set; }
        public int MemoryFactor { get; set; }
        public int MemoryLimitInKiloBytes { get; set; }

        public string[] Tags { get; set; }

        public Difficulty Difficulty { get; set; }

        public string ProblemSetId { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public string CourseId { get; set; }

        public string[] Hints { get; set; }
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}