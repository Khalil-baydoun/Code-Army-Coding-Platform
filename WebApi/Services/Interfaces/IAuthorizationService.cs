
namespace WebApi.Services.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> IsMemberOfCourse(string courseId, string userEmail, string role);

        Task<bool> IsAuthorizedToCourse(string courseId, string userEmail, string role);

        Task<bool> IsOwnerOfProblem(string problemId, string userEmail, string role);

        Task<bool> CanAccessProblem(string problemId, string userEmail);
    }
}
