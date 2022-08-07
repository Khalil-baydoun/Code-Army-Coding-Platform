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

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Course course = (Course)obj;

            return Id.Equals(course.Id) &&
                Name.Equals(course.Name) &&
                Description.Equals(course.Description) &&
                AuthorEmail.Equals(course.AuthorEmail);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Id);
            hashCode.Add(Name);
            hashCode.Add(AuthorEmail);
            hashCode.Add(Description);
            return hashCode.ToHashCode();
        }
    }
}