using System.Security.Claims;

namespace WebApi.Services.Interfaces
{
    public interface IAuthorizationService
    {
        bool IsMemberOfCourse(string courseId, ClaimsPrincipal User);

        bool IsAuthorizedToCourse(string courseId, ClaimsPrincipal User);

        bool IsAuthorizedToProblem(string problemId, ClaimsPrincipal User);
    }
}
