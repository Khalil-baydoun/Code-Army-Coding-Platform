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

        public int TimeLimitInMilliseconds { get; set; }

        public int MemoryLimitInKiloBytes { get; set; }

        public string[] Tags { get; set; }

        public string[] Hints { get; set; }

        public Difficulty Difficulty { get; set; }

        public bool IsPublic { get; set; }
    }
}