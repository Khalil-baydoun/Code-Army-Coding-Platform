using System.Security.Claims;
using DataContracts.Problems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interfaces;
using DataContracts.Submissions;
using Webapi.JudgingQueue;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProblemController : ControllerBase
    {
        private readonly ILogger<ProblemController> _logger;
        private readonly IProblemService _problemSevice;
        private readonly Services.Interfaces.IAuthorizationService _authorizationService;
        private readonly ISolutionService _solutionService;
        private readonly GlobalMapper _mapper;
        private readonly ISubmissionQueue _queue;

        public ProblemController(
            ISubmissionQueue queue,
            Services.Interfaces.IAuthorizationService authorizationService,
            GlobalMapper mapper,
            ILogger<ProblemController> logger,
            IProblemService problemSevice,
            ISolutionService solutionService)
        {
            _queue = queue;
            _authorizationService = authorizationService;
            _mapper = mapper;
            _logger = logger;
            _problemSevice = problemSevice;
            _solutionService = solutionService;
        }

        [HttpPost]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddProblem([FromBody] CreateProblemRequest req)
        {
            var problem = _mapper.ToProblem(req);
            problem.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
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

        [HttpGet("public")]
        [Authorize]
        public IActionResult GetPublicProblems()
        {
            var problem = _problemSevice.GetPublicProblems();
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
        [Authorize(Policy = "Admins&Instructors")]
        public IActionResult GetProblems()
        {
            var problems = _problemSevice.GetProblems(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value);
            return Ok(problems);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> UpdateProblem(string id, [FromBody] UpdateProblemRequest req)
        {
            if (!await _authorizationService.IsOwnerOfProblem(id, User))
            {
                return Forbid();
            }
            var problem = _mapper.ToProblem(req);
            problem.Id = int.Parse(id);
            problem.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            await _problemSevice.UpdateProblem(problem);
            return Ok();
        }


        [HttpPost("solution")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddSolution([FromForm] SolutionRequest solutionRequest)
        {
            if (!await _authorizationService.IsOwnerOfProblem(solutionRequest.ProblemId, User))
            {
                return Forbid();
            }
            var solution = await ToSubmissionRequest(solutionRequest);

            solution.IsSolution = true;
            solution.UserEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            await _queue.EnqueueSubmission(solution);
            return Ok();
        }

        [HttpGet("solution/{problemId}")]
        public async Task<IActionResult> GetSolution(string problemId, [FromQuery] string progLang)
        {
            if (!await _authorizationService.IsOwnerOfProblem(problemId, User))
            {
                return Forbid();
            }

            var solution = await _solutionService.GetSolution(problemId, progLang);
            return Ok(solution);
        }

        private static async Task<SubmissionRequest> ToSubmissionRequest(SolutionRequest solutionRequest)
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

        private static async Task<string> ReadAllFileAsync(IFormFile file)
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