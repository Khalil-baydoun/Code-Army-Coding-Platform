using DataContracts.Courses;
using webapi.Services.Interfaces;
using webapi.Store.Interfaces;

namespace WebApi.Services.Implementations
{
    public class CourseService : ICourseService
    {
        ICourseStore _courseStore;
        public CourseService(ICourseStore courseStore)
        {
            _courseStore = courseStore;
        }

        public Course GetCourse(string courseId)
        {
            return _courseStore.GetCourse(courseId);
        }

        public async Task<string> AddCourse(Course course)
        {
            return await _courseStore.AddCourse(course);
        }

        public List<Course> GetCourses(List<string> coursesIds)
        {
            return _courseStore.GetCourses(coursesIds);
        }

        public async Task AddUserToCourse(int courseId, string userEmail)
        {
            await _courseStore.AddUserToCourse(courseId, userEmail);
        }

        public async Task AddUsersToCourse(int courseId, List<string> usersEmails)
        {
            await _courseStore.AddUsersToCourse(courseId, usersEmails);
        }

        public bool IsOwner(string courseId, string userEmail)
        {
            return _courseStore.IsOwner(courseId, userEmail);
        }

        public bool IsMember(string courseId, string userEmail)
        {
            return _courseStore.IsMember(courseId, userEmail);
        }

        public async Task UpdateCourse(Course course)
        {
            await _courseStore.UpdateCourse(course);
        }

        public async Task DeleteCourse(string courseId)
        {
            await _courseStore.DeleteCourse(courseId);
        }
    }
}