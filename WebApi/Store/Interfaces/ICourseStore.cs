using System.Collections.Generic;
using System.Threading.Tasks;
using DataContracts.Courses;
using DataContracts.Groups;
using DataContracts.ProblemSets;

namespace webapi.Store.Interfaces
{
    public interface ICourseStore
    {
        Course GetCourse(string courseId);

        List<Course> GetCourses(List<string> coursesIds);

        Task<string> AddCourse(Course course);

        Task AddUserToCourse(int courseId, string userEmail);

        Task AddUsersToCourse(int courseId, List<string> usersEmails);

        bool IsOwner(string courseId, string userEmail);

        bool IsMember(string courseId, string userEmail);

        Task UpdateCourse(Course course);
        Task DeleteCourse(string courseId);
        List<Group> GetGroups(string courseId);
    }
}
