using System;
using System.Collections.Generic;
using System.Security.Claims;
using DataContracts.ProblemSets;
using DataContracts.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webapi.Services.Interfaces;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<SubmissionController> _logger;

        private readonly ICourseService _courseService;

        private readonly IProblemService _problemService;

        private readonly IProblemSetService _problemSetService;

        private readonly IStatisticsService _statisticsService;

        private readonly WebApi.Services.Interfaces.IAuthorizationService _authorizationService;

        public StatisticsController(WebApi.Services.Interfaces.IAuthorizationService authorizationService, IProblemSetService problemSetService, ICourseService courseService, IProblemService problemSevice, IStatisticsService statisticsService, ILogger<SubmissionController> logger)
        {
            _authorizationService = authorizationService;
            _courseService = courseService;
            _problemService = problemSevice;
            _statisticsService = statisticsService;
            _logger = logger;
            _problemSetService = problemSetService;
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
        public IActionResult GetCourseUserStatistics(int courseId)
        {
            if (!_authorizationService.IsAuthorizedToCourse(courseId.ToString(), User))
            {
                return Forbid();
            }
            var course = _courseService.GetCourse(courseId.ToString());
            List<UserStatistics> userStats = new List<UserStatistics>();
            foreach (var userEmail in course.UsersEmails)
            {
                userStats.Add(_statisticsService.GetUserStatisticsInCourse(userEmail, courseId));
            }
            return Ok(userStats);
        }

        [HttpGet("problemSet/{problemSetId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public IActionResult GetProblemSetStatistics(int problemSetId, int groupId = -1, DateTime? date = null)
        {
            ProblemSet problemSet = _problemSetService.GetProblemSet(problemSetId.ToString());
            if (!_authorizationService.IsAuthorizedToCourse(problemSet.CourseId.ToString(), User))
            {
                return Forbid();
            }
            var course = _courseService.GetCourse(problemSet.CourseId.ToString());
            var psStats = new ProblemSetStatistics();
            psStats.ProblemsStatistics = new List<ProblemStatistics>();
            psStats.UserStatistics = new List<UserProblemSetStatistics>();
            foreach (var userEmail in course.UsersEmails)
            {
                psStats.UserStatistics.Add(_statisticsService.GetProblemSolvedOfUserInProblemSet(problemSetId, userEmail, groupId));
            }
            foreach (var problem in problemSet.Problems)
            {
                psStats.ProblemsStatistics.Add(_statisticsService.GetProblemStatistics(problem.Id, groupId, problemSetId));
            }
            return Ok(psStats);
        }

        [HttpGet("problem/{problemId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public IActionResult GetProblemStatistics(int problemId)
        {
            if (!_authorizationService.IsAuthorizedToProblem(problemId.ToString(), User))
            {
                return Forbid();
            }
            var stats = _statisticsService.GetProblemStatistics(problemId);
            return Ok(stats);
        }
    }

}
