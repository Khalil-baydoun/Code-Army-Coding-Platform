using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DataContracts.Statistics;
using DataContracts.Submissions;
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
    public class SubmissionController : ControllerBase
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly IOnlineJudgeService _judgeSevice;
        private readonly IStatisticsService _statisticsService;
        private readonly IProblemService _problemService;
        private readonly IReportService _reportService;
        private readonly WebApi.Services.Interfaces.IAuthorizationService _authorizationService;
        private readonly JudgingQueue _queue;

        public SubmissionController(JudgingQueue queue, 
            WebApi.Services.Interfaces.IAuthorizationService authorizationService, 
            IProblemService problemService, 
            IStatisticsService statisticsService, 
            ILogger<SubmissionController> logger, 
            IOnlineJudgeService judgeSevice,
            IReportService reportService)
        {
            _queue = queue;
            _authorizationService = authorizationService;
            _problemService = problemService;
            _statisticsService = statisticsService;
            _logger = logger;
            _judgeSevice = judgeSevice;
            _reportService = reportService;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Submit([FromBody] SubmissionRequest submissionRequest)
        {
            //var courseId = _problemService.GetCourseIdOfProblem(submissionRequest.ProblemId);
            //if (!_authorizationService.IsMemberOfCourse(courseId, User))
            //{
            //    return Forbid();
            //}
            //_queue.Enqueue(submissionRequest, false, User);

            await _judgeSevice.JudgeCode(submissionRequest);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetSubmissions(int offset, int limit)
        {
            var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            var resp = _statisticsService.GetUserSubmissions(email, offset, limit + 1);
            if (resp.Submissions.Count == limit + 1)
            {
                resp.Submissions.RemoveAt(resp.Submissions.Count - 1);
                resp.SubmissionsRemaining = true;
            }
            else
            {
                resp.SubmissionsRemaining = false;
            }
            return Ok(resp);
        }

        [HttpGet("{submissionId}")]
        [Authorize]
        public IActionResult GetReport(string submissionId)
        {
            var report = _reportService.GetReport(submissionId);
            return Ok(report);
        }
    }
}
