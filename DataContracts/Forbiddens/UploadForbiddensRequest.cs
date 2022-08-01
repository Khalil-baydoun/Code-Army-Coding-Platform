using Microsoft.AspNetCore.Http;

namespace DataContracts.Forbiddens
{
    public class UploadForbiddensRequest
    {
        public IFormFile Keywords { get; set; }
        public string ProblemId { get; set; }
    }
}