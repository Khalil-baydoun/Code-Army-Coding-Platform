using System.Text;
using DataContracts.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interfaces;

using static Utilities.HelperFunctions;
namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Admins&Instructors")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly ITestService _testService;
        private readonly Services.Interfaces.IAuthorizationService _authorizationService;

        public TestController(Services.Interfaces.IAuthorizationService authorizationService, IProblemService problemService, ILogger<TestController> logger, ITestService testService)
        {
            _authorizationService = authorizationService;
            _logger = logger;
            _testService = testService;
        }

        [HttpGet("{problemId}")]
        public async Task<IActionResult> GetTests(string problemId)
        {
            if (!await _authorizationService.IsOwnerOfProblem(problemId, GetEmail(User), GetRole(User))) // instructor cannot get tests of problem that he/she is not the author of
            {
                return Forbid();
            }

            var tests = _testService.GetTestsOfProblem(problemId);
            return Ok(tests);
        }

        [HttpPost]
        public async Task<IActionResult> AddTest([FromBody] TestUnit test)
        {
            if (!await _authorizationService.IsOwnerOfProblem(test.ProblemId, GetEmail(User), GetRole(User))) // instructor cannot upload tests to problem that he/she is not the author of
            {
                return Forbid();
            }

            await _testService.AddTest(test.ProblemId, test);
            return Ok();
        }

        [HttpPost("uploadTests")]
        public async Task<IActionResult> UploadTests([FromForm] UploadTestsRequest uploadTestsRequest)
        {
            if (!await _authorizationService.IsOwnerOfProblem(uploadTestsRequest.ProblemId, GetEmail(User), GetRole(User))) // instructor cannot upload tests to problem that he/she is not the author of
            {
                return Forbid();
            }

            var tests = await ToTestUnits(uploadTestsRequest);
            _testService.AddTestBatch(tests);
            return Ok();
        }

        [HttpDelete("{problemId}")]
        public async Task<IActionResult> DeleteProblemTests(string problemId)
        {
            if (!await _authorizationService.IsOwnerOfProblem(problemId, GetEmail(User), GetRole(User))) // instructor cannot get tests of problem that he/she is not the author of
            {
                return Forbid();
            }

            await _testService.DeleteProblemTests(problemId);
            return Ok();
        }

        private async Task<List<TestUnit>> ToTestUnits(UploadTestsRequest uploadTestsRequest)
        {
            var input = await ReadFileAsync(uploadTestsRequest.Input);
            var output = await ReadFileAsync(uploadTestsRequest.Output);
            var tests = new List<TestUnit>();

            for (int i = 0; i < input.Count; i++)
            {
                tests.Add(new TestUnit
                {
                    Input = input[i],
                    Output = output[i],
                    ProblemId = uploadTestsRequest.ProblemId
                });
            }

            return tests;
        }

        public static async Task<List<string>> ReadFileAsync(IFormFile file)
        {
            var result = new List<string>();
            var builder = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var input = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(input))
                    {
                        builder.AppendLine(input);
                    }
                    else
                    {
                        if (builder.Length > 0)
                        {
                            result.Add(builder.ToString());
                            builder = builder.Clear();
                        }
                    }
                }
            }

            if (builder.Length > 0)
            {
                result.Add(builder.ToString());
            }

            return result;
        }
    }
}