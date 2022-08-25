using System.ComponentModel.DataAnnotations;

namespace DataContracts.ProblemSets
{
    public class AddProblemSetRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AuthorEmail { get; set; }

        public string Description { get; set; }

        public string[] Prerequisites { get; set; }

        public List<string> ProblemIds { get; set; }

        public DateTime? DueDate { get; set; }

        public int CourseId { get; set; }
    };
}