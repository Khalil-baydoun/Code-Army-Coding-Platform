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

        public async Task<bool> IsAuthorizedToCourse(string courseId, string userEmail, string role)
        {
            if (role.Equals("Instructor"))
            {
                return await _courseService.IsOwner(courseId, userEmail);
            }

            return true;
        }

        public async Task<bool> IsOwnerOfProblem(string problemId, string userEmail, string role)
        {
            if (role.Equals("Instructor"))
            {
                return await _problemService.IsOwner(problemId, userEmail);
            }

            return true;
        }

        public async Task<bool> CanAccessProblem(string problemId, string userEmail)
        {
            return await _problemService.CanAccessProblem(problemId, userEmail);
        }

        public async Task<bool> IsMemberOfCourse(string courseId, string userEmail, string role)
        {
            if (!role.Equals("Admin"))
            {
                return _courseService.IsMember(courseId, userEmail) || await _courseService.IsOwner(courseId, userEmail);
            }

            return true;
        }
    }
}