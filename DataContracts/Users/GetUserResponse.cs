using System.ComponentModel.DataAnnotations;
using DataContracts.Courses;

namespace DataContracts.Users
{
    public class GetUserResponse
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public Role? Role { get; set; }

        public List<Course> Courses { get; set; }

        public List<Course> OwnedCourses { get; set; }

        protected bool Equals(GetUserResponse other)
        {
            return Email == other.Email &&
                   FirstName == other.FirstName && LastName == other.LastName && Role == other.Role;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GetUserResponse)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Email);
            hashCode.Add(FirstName);
            hashCode.Add(LastName);
            hashCode.Add(Role);
            return hashCode.ToHashCode();
        }
    }
}