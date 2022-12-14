using System.Security.Claims;
using DataContracts.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities;
using webapi.Services.Interfaces;
using static Utilities.HelperFunctions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        private readonly Services.Interfaces.IAuthorizationService _authorizationService;

        private readonly GlobalMapper _mapper;

        public CourseController(Services.Interfaces.IAuthorizationService authorizationService, GlobalMapper mapper, ICourseService courseService)
        {
            _authorizationService = authorizationService;
            _mapper = mapper;
            _courseService = courseService;
        }

        [HttpPost]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddCourse([FromBody] Course course)
        {
            course.AuthorEmail = GetEmail(User);
            var id = await _courseService.AddCourse(course);
            return Ok(new { CourseId = id.ToString() });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] UpdateCourseRequest req)
        {
            if (!await IsAuthorizedToCourse(id, User))
            {
                return Forbid();
            }

            var course = _mapper.ToCourse(req);
            course.Id = int.Parse(id);
            course.AuthorEmail = GetEmail(User);
            await _courseService.UpdateCourse(course);
            return Ok();
        }

        [HttpGet("{CourseId}")]
        [Authorize]
        public async Task<IActionResult> GetCourse(string courseId)
        {
            if (!await _authorizationService.IsMemberOfCourse(courseId, GetEmail(User), GetRole(User)))
            {
                return Forbid();
            }
            Course course = _courseService.GetCourse(courseId);
            return Ok(course);
        }

        [HttpPost("addusers")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddUsersToCourse([FromBody] UpdateCourseUsersRequest addUsersReq)
        {
            if (!await IsAuthorizedToCourse(addUsersReq.CourseId, User))
            {
                return Forbid();
            }

            await _courseService.AddUsersToCourse(int.Parse(addUsersReq.CourseId), addUsersReq.UserEmails);
            return Ok();
        }

        [HttpPost("removeusers")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> RemoveUsersFromCourse([FromBody] UpdateCourseUsersRequest removeUsersReq)
        {
            if (!await IsAuthorizedToCourse(removeUsersReq.CourseId, User))
            {
                return Forbid();
            }

            await _courseService.RemoveUsersFromCourse(int.Parse(removeUsersReq.CourseId), removeUsersReq.UserEmails);
            return Ok();
        }

        [HttpDelete("{CourseId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            if (!await IsAuthorizedToCourse(courseId, User))
            {
                return Forbid();
            }

            await _courseService.DeleteCourse(courseId);
            return Ok();
        }

        private async Task<bool> IsAuthorizedToCourse(string courseId, ClaimsPrincipal User)
        {
            return await _authorizationService.IsAuthorizedToCourse(courseId, GetEmail(User), GetEmail(User));
        }
    }
}

