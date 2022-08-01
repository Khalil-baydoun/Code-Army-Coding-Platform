using Microsoft.AspNetCore.Http;
namespace DataContracts.Submissions
{
    public class SolutionRequest
    {
        public IFormFile SourceCode { get; set; }

        public ProgrammingLanguage ProgLanguage { get; set; }

        public string ProblemId { get; set; }
    }
}