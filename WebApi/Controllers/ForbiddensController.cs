using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataContracts.Forbiddens;
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

    public class ForbiddensController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private readonly IForbiddensService _forbiddensService;
        private readonly IProblemService _problemService;

        public ForbiddensController(IProblemService problemService, ILogger<TestController> logger, IForbiddensService forbiddensService)
        {
            _problemService = problemService;
            _logger = logger;
            _forbiddensService = forbiddensService;
        }

        [HttpGet("{problemId}")]
        public IActionResult GetForbiddens(string problemId)
        {
            if (!isAuthorizedToProblem(problemId)) // instructor cannot get forbidden keywords of problem that he/she is not the author of
            {
                return Forbid();
            }
            var forbiddens = _forbiddensService.GetForbiddensOfProblem(problemId);
            return Ok(forbiddens);
        }


        [HttpPost]
        public IActionResult AddForbiddens([FromBody] Forbiddens forbiddnes)
        {
            if (!isAuthorizedToProblem(forbiddnes.ProblemId)) // instructor cannot upload tests to problem that he/she is not the author of
            {
                return Forbid();
            }
            _forbiddensService.AddForbiddens(forbiddnes.ProblemId, forbiddnes);
            return Ok();
        }

        [HttpPost("uploadForbiddens")]
        public async Task<IActionResult> UploadForbiddens([FromForm] UploadForbiddensRequest uploadForbiddensRequest)
        {
            if (!isAuthorizedToProblem(uploadForbiddensRequest.ProblemId)) // instructor cannot upload tests to problem that he/she is not the author of
            {
                return Forbid();
            }
            var forbiddens = await ToForbiddens(uploadForbiddensRequest);
            await _forbiddensService.AddForbiddens(uploadForbiddensRequest.ProblemId, forbiddens);
            return Ok();
        }


        private async Task<Forbiddens> ToForbiddens(UploadForbiddensRequest uploadForbiddensRequest)
        {
            var input = await ReadFileAsync(uploadForbiddensRequest.Keywords);
            var forbiddnes = new Forbiddens();

            forbiddnes.ProblemId = uploadForbiddensRequest.ProblemId;
            forbiddnes.Keywords = string.Join(",",input.ToArray());
            return forbiddnes;
        }


        bool isAuthorizedToProblem(string problemId)
        {
            if (User.IsInRole("Instructor")) // instructor cannot upload tests to problem that he/she is not the author of
            {
                var userEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
                return _problemService.IsOwner(problemId, userEmail);
            }
            return true;
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