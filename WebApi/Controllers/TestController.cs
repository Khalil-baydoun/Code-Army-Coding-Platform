using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataContracts.Tests;
using DataContracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Admins&Instructors")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private readonly ITestService _testService;
        private readonly IProblemService _problemService;
        private readonly WebApi.Services.Interfaces.IAuthorizationService _authorizationService;

        public TestController(WebApi.Services.Interfaces.IAuthorizationService authorizationService, IProblemService problemService, ILogger<TestController> logger, ITestService testService)
        {
            _authorizationService = authorizationService;
            _problemService = problemService;
            _logger = logger;
            _testService = testService;
        }

        [HttpGet("{problemId}")]
        public IActionResult GetTests(string problemId)
        {
            if (!_authorizationService.IsAuthorizedToProblem(problemId, User)) // instructor cannot get tests of problem that he/she is not the author of
            {
                return Forbid();
            }
            var tests = _testService.GetTestsOfProblem(problemId);
            return Ok(tests);
        }

        [HttpPost]
        public async Task<IActionResult> AddTest([FromBody] TestUnit test)
        {
            if (!_authorizationService.IsAuthorizedToProblem(test.ProblemId, User)) // instructor cannot upload tests to problem that he/she is not the author of
            {
                return Forbid();
            }
            await _testService.AddTest(test.ProblemId, test);
            return Ok();
        }

        [HttpPost("uploadTests")]
        public async Task<IActionResult> UploadTests([FromForm] UploadTestsRequest uploadTestsRequest)
        {
            if (!_authorizationService.IsAuthorizedToProblem(uploadTestsRequest.ProblemId, User)) // instructor cannot upload tests to problem that he/she is not the author of
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
            if (!_authorizationService.IsAuthorizedToProblem(problemId, User)) // instructor cannot get tests of problem that he/she is not the author of
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