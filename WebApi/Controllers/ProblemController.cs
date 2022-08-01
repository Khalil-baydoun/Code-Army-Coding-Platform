using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataContracts.Problems;
using DataContracts.ProblemSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webapi.Services.Interfaces;
using WebApi.Services.Interfaces;
using DataContracts.Submissions;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProblemController : ControllerBase
    {
        private readonly ILogger<ProblemController> _logger;

        private readonly IProblemService _problemSevice;

        private readonly IProblemSetService _problemSetService;

        private readonly ICourseService _courseService;

        private readonly IOnlineJudgeService _judgeSevice;

        private readonly Services.Interfaces.IAuthorizationService _authorizationService;

        private readonly ISolutionService _solutionService;

        private readonly GlobalMapper _mapper;

        private readonly JudgingQueue _queue;

        public ProblemController(
            JudgingQueue queue,
            Services.Interfaces.IAuthorizationService authorizationService,
            ICourseService courseService,
            IProblemSetService problemSetSevice,
            GlobalMapper mapper,
            ILogger<ProblemController> logger,
            IProblemService problemSevice,
            IOnlineJudgeService judgeSevice,
            ISolutionService solutionService)
        {
            _queue = queue;
            _authorizationService = authorizationService;
            _problemSetService = problemSetSevice;
            _courseService = courseService;
            _mapper = mapper;
            _logger = logger;
            _problemSevice = problemSevice;
            _judgeSevice = judgeSevice;
            _solutionService = solutionService;
        }

        [HttpPost]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddProblem([FromBody] CreateProblemRequest req)
        {
            ProblemSet problemSet = _problemSetService.GetProblemSet(req.ProblemSetId);
            if (!_authorizationService.IsAuthorizedToCourse(problemSet.CourseId.ToString(), User))
            {
                return Forbid();
            }
            var problem = _mapper.ToProblem(req);
            problem.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            if (req.ProblemSetId != null) problem.ProblemSetId = req.ProblemSetId;
            var id = await _problemSevice.AddProblem(problem);
            return Ok(new { ProblemId = id.ToString() });
        }

        [HttpGet("{problemId}")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetProblem(string problemId)
        {
            var problem = _problemSevice.GetProblem(problemId);
            return Ok(problem);
        }

        [HttpDelete("{problemId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> DeleteProblem(string problemId)
        {
            await _problemSevice.DeleteProblem(problemId);
            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult GetProblems()
        {
            var problems = _problemSevice.GetProblems();
            return Ok(problems);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> UpdateProblem(string id, [FromBody] UpdateProblemRequest req)
        {
            if (!_authorizationService.IsAuthorizedToProblem(id, User))
            {
                return Forbid();
            }
            var problem = _mapper.ToProblem(req);
            problem.Id = Int32.Parse(id);
            problem.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            await _problemSevice.UpdateProblem(problem);
            return Ok();
        }


        [HttpPost("solution")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddSolution([FromForm] SolutionRequest solutionRequest)
        {
            if (!_authorizationService.IsAuthorizedToProblem(solutionRequest.ProblemId, User))
            {
                return Forbid();
            }
            var solution = await ToSubmissionRequest(solutionRequest);
            _queue.Enqueue(solution, true, User);
            return Ok();
        }

        [HttpGet("solution/{problemId}")]
        public async Task<IActionResult> GetSolution(string problemId, [FromQuery] int ProgLang)
        {
            if (!_authorizationService.IsAuthorizedToProblem(problemId, User))
            {
                return Forbid();
            }
            var solution = await _solutionService.GetSolution(problemId, (ProgrammingLanguage)ProgLang);
            return Ok(solution);
        }

        private async Task<SubmissionRequest> ToSubmissionRequest(SolutionRequest solutionRequest)
        {
            var sourceCode = await ReadAllFileAsync(solutionRequest.SourceCode);
            var submissionRequest = new SubmissionRequest
            {
                SourceCode = sourceCode,
                ProblemId = solutionRequest.ProblemId,
                ProgLanguage = solutionRequest.ProgLanguage,
            };
            return submissionRequest;
        }

        private async Task<string> ReadAllFileAsync(IFormFile file)
        {
            string result;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                result = await reader.ReadToEndAsync();

            }
            return result;
        }
    }
}