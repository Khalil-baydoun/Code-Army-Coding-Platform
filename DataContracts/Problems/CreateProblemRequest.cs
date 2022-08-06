using DataContracts.Submissions;
using DataContracts.Tests;

namespace DataContracts.Problems
{
    public class CreateProblemRequest
    {
        public string Title { get; set; }

        public string AuthorEmail { get; set; }

        public string GeneralDescription { get; set; }

        public string InputDescription { get; set; }

        public string OutputDescription { get; set; }

        public string SampleInput { get; set; }

        public string SampleOutput { get; set; }

        // public int TimeLimitInMilliseconds { get; set; } //to factor
        public int TimeFactor { get; set; }
        public int MemoryFactor { get; set; }

        //public int MemoryLimitInKiloBytes { get; set; }  //to factor

        public string[] Tags { get; set; }

        public string[] Hints { get; set; }

        public Difficulty Difficulty { get; set; }

        public string CourseId { get; set; }

        public string ProblemSetId { get; set; }
    }
}