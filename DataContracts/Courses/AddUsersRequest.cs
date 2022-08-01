using Microsoft.AspNetCore.Http;
namespace DataContracts.Courses
{
    public class AddUsersRequest
    {
        public IFormFile Users { get; set; }

        public string CourseId { get; set; }
    }
}