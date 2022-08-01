using System;
using System.Collections.Generic;

namespace DataContracts.Courses
{
    public class UpdateCourseRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    };
}