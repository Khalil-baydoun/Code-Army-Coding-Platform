using Microsoft.AspNetCore.Http;
namespace DataContracts.Courses
{
    public class UpdateCourseUsersRequest
    {
        public List<string> UserEmails { get; set; }

        public string CourseId { get; set; }
    }
}