using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Store.Interfaces;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly WebApi.Services.Interfaces.IAuthorizationService _authorizationService;

        public StatisticsController(WebApi.Services.Interfaces.IAuthorizationService authorizationService, IStatisticsService statisticsService)
        {
            _authorizationService = authorizationService;
            _statisticsService = statisticsService;
        }

        [HttpGet("user")]
        public IActionResult GetUserStatistics()
        {
            var userEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            var stats = _statisticsService.GetUserStatistics(userEmail);
            return Ok(stats);
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> GetCourseStatistics(int courseId)
        {
            if (!await _authorizationService.IsAuthorizedToCourse(courseId.ToString(), User))
            {
                return Forbid();
            }
            var courseStats = _statisticsService.GetCourseStatistics(courseId);
            return Ok(courseStats);
        }

        [HttpGet("problem/{problemId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> GetProblemStatistics(int problemId)
        {
            if (!await _authorizationService.IsOwnerOfProblem(problemId.ToString(), User))
            {
                return Forbid();
            }

            var stats = _statisticsService.GetProblemStatistics(problemId);
            return Ok(stats);
        }
    }
}
