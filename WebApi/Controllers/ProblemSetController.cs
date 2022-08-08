using System.Security.Claims;
using DataContracts.ProblemSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Utilities.HelperFunctions;
using Utilities;
using webapi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProblemSetController : Controller
    {
        private readonly IProblemSetService _problemSetService;
        private readonly Services.Interfaces.IAuthorizationService _authorizationService;
        private readonly GlobalMapper _mapper;

        public ProblemSetController(Services.Interfaces.IAuthorizationService authorizationService, GlobalMapper mapper, IProblemSetService problemSetService)
        {
            _authorizationService = authorizationService;
            _mapper = mapper;
            _problemSetService = problemSetService;
        }

        [HttpPost]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddProblemSet([FromBody] AddProblemSetRequest problemSet)
        {
            problemSet.AuthorEmail = GetEmail(User);
            if (!await IsAuthorizedToCourse(problemSet.CourseId.ToString(), User))
            {
                return Forbid();
            }

            var id = await _problemSetService.AddProblemSet(problemSet);
            return Ok(new { ProblemSetId = id.ToString() });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> UpdateProblemSet(string id, [FromBody] UpdateProblemSetRequest req)
        {
            var ps = _problemSetService.GetProblemSet(id);
            if (!await IsAuthorizedToCourse(ps.CourseId.ToString(), User))
            {
                return Forbid();
            }
            var problemSet = _mapper.ToProblemSet(req);
            problemSet.Id = int.Parse(id);
            problemSet.CourseId = ps.CourseId;
            await _problemSetService.UpdateProblemSet(problemSet);
            return Ok();
        }

        [HttpGet("{problemSetId}")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetProblemSet(string problemSetId)
        {
            ProblemSet problemSet = _problemSetService.GetProblemSet(problemSetId);
            return Ok(problemSet);
        }

        [HttpDelete("{problemSetId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> DeleteProblemSet(string problemSetId)
        {
            await _problemSetService.DeleteProblemSet(problemSetId);
            return Ok();
        }

        [HttpPost("addproblem")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddProblemToProblemSet([FromBody] AddProblemToProblemSetRequest addRequest)
        {
            ProblemSet problemSet = _problemSetService.GetProblemSet(addRequest.ProblemSetId.ToString());
            if (!await IsAuthorizedToCourse(problemSet.CourseId.ToString(), User)
                || ! await _authorizationService.CanAccessProblem(addRequest.ProblemId.ToString(), GetEmail(User)))
            {
                return Forbid();
            }
            await _problemSetService.AddProblemToProblemSet(addRequest.ProblemSetId, addRequest.ProblemId);
            return Ok();
        }

        private async Task<bool> IsAuthorizedToCourse(string courseId, ClaimsPrincipal User)
        {
            return await _authorizationService.IsAuthorizedToCourse(courseId, GetEmail(User), GetRole(User));
        }
    }
}

