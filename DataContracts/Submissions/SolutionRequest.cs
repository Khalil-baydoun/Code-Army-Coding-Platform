using Microsoft.AspNetCore.Http;

namespace DataContracts.Submissions
{
    public class SolutionRequest
    {
        public IFormFile SourceCode { get; set; }

        public string ProgLanguage { get; set; }

        public string ProblemId { get; set; }
    }
}