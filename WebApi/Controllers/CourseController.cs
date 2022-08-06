using System.Security.Claims;
using DataContracts.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Services.Interfaces;

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
            course.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            var id = await _courseService.AddCourse(course);
            return Ok(new { CourseId = id.ToString() });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] UpdateCourseRequest req)
        {
            if (!_authorizationService.IsAuthorizedToCourse(id, User))
            {
                return Forbid();
            }
            var course = _mapper.ToCourse(req);
            course.Id = Int32.Parse(id);
            course.AuthorEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
            await _courseService.UpdateCourse(course);
            return Ok();
        }

        [HttpGet("{CourseId}")]
        public IActionResult GetCourse(string courseId)
        {
            if (!_authorizationService.IsMemberOfCourse(courseId, User))
            {
                return Forbid();
            }
            Course course = _courseService.GetCourse(courseId);
            return Ok(course);
        }

        [HttpPost("adduser")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddUserToCourse([FromBody] AddUserToCourseRequest addUserToCourseRequest)
        {
            if (!_authorizationService.IsAuthorizedToCourse(addUserToCourseRequest.CourseId.ToString(), User))
            {
                return Forbid();
            }
            await _courseService.AddUserToCourse(addUserToCourseRequest.CourseId, addUserToCourseRequest.UserEmail);
            return Ok();
        }

        [HttpPost("addusers")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> AddUsersToCourse([FromForm] AddUsersRequest addUsersReq)
        {
            if (!_authorizationService.IsAuthorizedToCourse(addUsersReq.CourseId, User))
            {
                return Forbid();
            }
            var userEmails = await ReadFileAsync(addUsersReq.Users);
            await _courseService.AddUsersToCourse(Int32.Parse(addUsersReq.CourseId), userEmails);
            return Ok();
        }

        [HttpDelete("{CourseId}")]
        [Authorize(Policy = "Admins&Instructors")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            await _courseService.DeleteCourse(courseId);
            return Ok();
        }

        public static async Task<List<string>> ReadFileAsync(IFormFile file)
        {
            var usersEmails = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var user = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(user))
                    {
                        usersEmails.Add(user);
                    }
                }
            }
            return usersEmails;
        }
    }
}

