using System.Collections.Generic;

namespace DataContracts.Courses
{
    public class AddUserToCourseRequest
    {
        public int CourseId { get; set; }

        public string UserEmail { get; set; }
    }
}