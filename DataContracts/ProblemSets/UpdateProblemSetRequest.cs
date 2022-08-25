namespace DataContracts.ProblemSets
{
    public class UpdateProblemSetRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Prerequisites { get; set; }

        public DateTime? DueDate { get; set; }
    };
}