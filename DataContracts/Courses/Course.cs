using System.Collections.Generic;
using DataContracts.ProblemSets;

namespace DataContracts.Courses
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AuthorEmail { get; set; }

        public string Description { get; set; }

        public List<string> UsersEmails { get; set; }

        public List<ProblemSet> ProblemSets { get; set; }
    }
}