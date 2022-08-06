using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DataContracts.ProblemSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProblemSetController : Controller
    {
        private readonly IProblemSetService _problemSetService;
        private readonly Services.Interfaces.IAuthorizationService _authorizationService;
        private readonly ICourseService _courseService;
        private readonly GlobalMapper _mapper;

        public ProblemSetController(Services.Interfaces.IAuthorizationService authorizationService, GlobalMapper mapper, IProblemSetService problemSetService, ICourseService courseService)
        {
            _authorizationService = authorizationService;
            _courseService = courseService;
            _mapper = mapper;
            _problemSetService = problemSetService;
        }

        [HttpPost]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddProblemSet([FromBody] AddProblemSetRequest problemSet)
        {
            problemSet.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            if (!_authorizationService.IsAuthorizedToCourse(problemSet.CourseId.ToString(), User))
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
            if (!_authorizationService.IsAuthorizedToCourse(ps.CourseId.ToString(), User))
            {
                return Forbid();
            }
            var problemSet = _mapper.ToProblemSet(req);
            problemSet.Id = Int32.Parse(id);
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
            if (!_authorizationService.IsAuthorizedToCourse(problemSet.CourseId.ToString(), User))
            {
                return Forbid();
            }
            await _problemSetService.AddProblemToProblemSet(addRequest.ProblemSetId, addRequest.ProblemId);
            return Ok();
        }
    }
}

