using System.Security.Claims;
using DataContracts.Submissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Services.Interfaces;
using Webapi.JudgingQueue;
using WebApi.Services.Interfaces;
using WebApi.Store.Interfaces;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController : ControllerBase
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly IStatisticsService _statisticsService;
        private readonly IWaReportService _reportService;
        private readonly WebApi.Services.Interfaces.IAuthorizationService _authorizationService;
        private readonly ISubmissionQueue _queue;

        public SubmissionController(
            ISubmissionQueue queue,
            WebApi.Services.Interfaces.IAuthorizationService authorizationService,
            IStatisticsService statisticsService,
            ILogger<SubmissionController> logger,
            IWaReportService reportService)
        {
            _queue = queue;
            _authorizationService = authorizationService;
            _statisticsService = statisticsService;
            _logger = logger;
            _reportService = reportService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] SubmissionRequest submissionRequest)
        {
            if (!await _authorizationService.CanSubmit(submissionRequest.ProblemId, User))
            {
                return Forbid();
            }

            submissionRequest.IsSolution = false;
            submissionRequest.UserEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;

            await _queue.EnqueueSubmission(submissionRequest);
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
