using System.Security.Claims;

namespace WebApi.Services.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> IsMemberOfCourse(string courseId, ClaimsPrincipal User);

        Task<bool> IsAuthorizedToCourse(string courseId, ClaimsPrincipal User);

        Task<bool> IsOwnerOfProblem(string problemId, ClaimsPrincipal User);

        Task<bool> CanSubmit(string problemId, ClaimsPrincipal User);
    }
}
