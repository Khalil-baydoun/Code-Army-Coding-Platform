
namespace DataContracts.Submissions
{
    public class SubmissionRequest
    {
        public string ProblemId { get; set; }

        public string? SubmissionId { get; set; }

        public string SourceCode { get; set; }

        public string ProgLanguage { get; set; }

        public bool IsSolution { get; set; }

        public string? UserEmail { get; set; }
    }
}