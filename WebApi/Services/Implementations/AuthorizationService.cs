using System.Security.Claims;
using webapi.Services.Interfaces;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ICourseService _courseService;
        private readonly IProblemService _problemService;

        public AuthorizationService(IProblemService problemService, ICourseService courseService)
        {
            _problemService = problemService;
            _courseService = courseService;
        }

        public bool IsAuthorizedToCourse(string courseId, ClaimsPrincipal User)
        {
            if (User.IsInRole("Instructor"))
            {
                var userEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
                return _courseService.IsOwner(courseId, userEmail);
            }
            return true;
        }

        public bool IsAuthorizedToProblem(string problemId, ClaimsPrincipal User)
        {
            if (User.IsInRole("Instructor"))
            {
                var userEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
                return _problemService.IsOwner(problemId, userEmail);
            }
            return true;
        }

        public bool IsMemberOfCourse(string courseId, ClaimsPrincipal User)
        {
            if (!User.IsInRole("Admin"))
            {
                var userEmail = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email).Value;
                return _courseService.IsMember(courseId, userEmail) || _courseService.IsOwner(courseId, userEmail);
            }
            return true;
        }
    }
}