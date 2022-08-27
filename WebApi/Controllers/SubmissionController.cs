using System.Security.Claims;
using DataContracts.Submissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webapi.JudgingQueue;
using WebApi.Services.Interfaces;
using static Utilities.HelperFunctions;
using WebApi.Store.Interfaces;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController : ControllerBase
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly ISubmissionService _submissionService;
        private readonly WebApi.Services.Interfaces.IAuthorizationService _authorizationService;
        private readonly ISubmissionQueue _queue;

        public SubmissionController(
            ISubmissionQueue queue,
            WebApi.Services.Interfaces.IAuthorizationService authorizationService,
            ILogger<SubmissionController> logger,
            ISubmissionService reportService)
        {
            _queue = queue;
            _authorizationService = authorizationService;
            _logger = logger;
            _submissionService = reportService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] SubmissionRequest submissionRequest)
        {
            if (!await _authorizationService.CanAccessProblem(submissionRequest.ProblemId, GetEmail(User)))
            {
                return Forbid();
            }

            submissionRequest.IsSolution = false;
            submissionRequest.UserEmail = GetEmail(User);

            await _queue.EnqueueSubmission(submissionRequest);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetSubmissions(int offset, int limit)
        {
            var email = GetEmail(User);
            var resp = _submissionService.GetUserSubmissions(email, offset, limit + 1);

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
    }
}
