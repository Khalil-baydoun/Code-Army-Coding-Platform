namespace DataContracts.Problems
{
    public class UpdateProblemRequest
    {
        public string Title { get; set; }

        public string GeneralDescription { get; set; }

        public string IDescription { get; set; }

        public string ODescription { get; set; }

        public string SampleInput { get; set; }

        public string SampleOutput { get; set; }

        public int TimeFactor { get; set; }

        public int MemoryFactor { get; set; }

        public string[] Tags { get; set; }

        public string[] Hints { get; set; }

        public Difficulty Difficulty { get; set; }
    }
}