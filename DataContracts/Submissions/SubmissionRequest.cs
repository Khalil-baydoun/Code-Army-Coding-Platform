using Microsoft.AspNetCore.Http;

namespace DataContracts.Submissions
{
    public class SubmissionRequest
    {
        public string ProblemId { get; set; }

        public string SourceCode { get; set; }

        public ProgrammingLanguage ProgLanguage { get; set; }
    }

    public enum ProgrammingLanguage
    {
        Cpp,
        Java,
        Python
    };
}