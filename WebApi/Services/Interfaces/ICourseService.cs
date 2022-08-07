using DataContracts.Courses;

namespace webapi.Services.Interfaces
{
    public interface ICourseService
    {
        Course GetCourse(string courseId);
        
        List<Course> GetCourses(List<string> coursesIds);
        
        Task<string> AddCourse(Course course);
        
        Task AddUserToCourse(int courseId, string userEmail);
        
        Task<bool> IsOwner(string courseId, string userEmail);
        
        bool IsMember(string courseId, string userEmail);
        
        Task UpdateCourse(Course course);
        
        Task AddUsersToCourse(int courseId, List<string> usersEmails);

        Task RemoveUsersFromCourse(int courseId, List<string> usersEmails);

        Task DeleteCourse(string courseId);
    }
}
