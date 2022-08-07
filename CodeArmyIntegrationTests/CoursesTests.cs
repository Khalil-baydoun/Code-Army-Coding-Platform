using DataContracts.Users;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestUtilities;
using Xunit;

namespace CodeArmyIntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class CoursesTests : CodeArmyTestBase
    {
        public CoursesTests(IntegrationTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task PostUpdateGetDeleteCourseInstructor()
        {
            TestUser instructor = await CreateUser(Role.Instructor);
            TestUser user1 = await CreateUser(Role.User);
            TestUser user2 = await CreateUser(Role.User);
            TestUser user3 = await CreateUser(Role.User);


            var course = TestsUtilities.CreateRandomCourse();
            string? courseId = await instructor.AddCourseAsync(course);

            course.Description = "Updated course description";
            await instructor.UpdateCourseAsync(courseId, description: course.Description);

            List<string> usersEmails = new()
            {
                user1.Email,
                user2.Email,
                user3.Email
            };

            List<string> usersEmailsToRemove = new() { user1.Email };

            await instructor.AddUsersToCourse(courseId, usersEmails);
            await instructor.RemoveUsersFromCourse(courseId, usersEmailsToRemove);

            var fetchedCourse = await instructor.GetCourseAsync(courseId);
            course.Id = int.Parse(courseId);
            course.AuthorEmail = instructor.Email;

            Assert.Equal(course, fetchedCourse);
            Assert.True(TestsUtilities.ListsEqual(fetchedCourse?.UsersEmails, usersEmails.Except(usersEmailsToRemove).ToList()));

            await instructor.DeleteCourseAsync(courseId);

            var ex = await Assert.ThrowsAsync<CodeArmyException>(() => instructor.GetCourseAsync(courseId));
            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task InstructorCantAccessOrModifyOtherInstructorCourse()
        {
            TestUser instructor1 = await CreateUser(Role.Instructor);
            TestUser instructor2 = await CreateUser(Role.Instructor);

            List<string> usersEmails = new()
            {
                "user1Email",
                "user2Email",
                "user3Email"
            };

            var course = TestsUtilities.CreateRandomCourse();
            string? courseId = await instructor1.AddCourseAsync(course);

            var ex = await Assert.ThrowsAsync<CodeArmyException>(() => instructor2.GetCourseAsync(courseId));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => instructor2.UpdateCourseAsync(courseId, description: "Update Attempt"));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => instructor2.DeleteCourseAsync(courseId));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => instructor2.AddUsersToCourse(courseId, usersEmails));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);

            ex = await Assert.ThrowsAsync<CodeArmyException>(() => instructor2.RemoveUsersFromCourse(courseId, usersEmails));
            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);
        }
    }
}
