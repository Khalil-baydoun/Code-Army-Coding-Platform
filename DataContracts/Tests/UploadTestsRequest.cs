using Microsoft.AspNetCore.Http;

namespace DataContracts.Tests
{
    public class UploadTestsRequest
    {
        public IFormFile Input { get; set; }

        public IFormFile Output { get; set; }

        public string ProblemId { get; set; }
    }
}