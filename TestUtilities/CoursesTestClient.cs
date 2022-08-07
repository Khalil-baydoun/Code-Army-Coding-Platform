using DataContracts.Courses;

namespace TestUtilities
{
    public static class CoursesTestClient
    {
        private static readonly string courseUrl = "course";

        public static async Task<string?> AddCourseAsync(this TestUser user, Course addCourseRequest)
        {
            var response = await user.SendRequestAsync(HttpMethod.Post, courseUrl, TestsUtilities.ToStringContent(addCourseRequest));
            return await TestsUtilities.ExtractIdFromResponse(response, "CourseId");
        }

        public static async Task DeleteCourseAsync(this TestUser user, string courseId)
        {
            await user.SendRequestAsync(HttpMethod.Delete, courseUrl + $"/{courseId}");
        }

        public static async Task<Course?> GetCourseAsync(this TestUser user, string courseId)
        {
            var response = await user.SendRequestAsync(HttpMethod.Get, courseUrl + $"/{courseId}");
            return await TestsUtilities.ExtractResponse<Course>(response);
        }

        public static async Task AddUsersToCourse(this TestUser user, string courseId, List<string> userEmails)
        {
            var addUsersToCourseRequest = new UpdateCourseUsersRequest
            {
                CourseId = courseId,
                UserEmails = userEmails
            };

            await user.SendRequestAsync(HttpMethod.Post, courseUrl + "/addusers", TestsUtilities.ToStringContent(addUsersToCourseRequest));
        }

        public static async Task RemoveUsersFromCourse(this TestUser user, string courseId, List<string> userEmails)
        {
            var removeUsersFromCourseRequest = new UpdateCourseUsersRequest
            {
                CourseId = courseId,
                UserEmails = userEmails
            };

            await user.SendRequestAsync(HttpMethod.Post, courseUrl + "/removeusers", TestsUtilities.ToStringContent(removeUsersFromCourseRequest));
        }

        public static async Task UpdateCourseAsync(this TestUser user, string? courseId, string? name = null, string? description = null)
        {
            var courseUpdateRequest = new UpdateCourseRequest()
            {
                Description = description,
                Name = name
            };

            await user.SendRequestAsync(HttpMethod.Put, courseUrl + $"/{courseId}", TestsUtilities.ToStringContent(courseUpdateRequest));
        }
    }
}
