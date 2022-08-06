using System.ComponentModel.DataAnnotations;

namespace DataContracts.Users
{
    public class AddUserRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public string Salt { get; set; }

        public Role? Role { get; set; }

        public List<string> CourseIds { get; set; }

        protected bool Equals(AddUserRequest other)
        {
            return Email == other.Email && Salt == other.Salt &&
                   FirstName == other.FirstName && LastName == other.LastName && Role == other.Role;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddUserRequest)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Email);
            hashCode.Add(Password);
            hashCode.Add(FirstName);
            hashCode.Add(LastName);
            hashCode.Add(Role);
            hashCode.Add(Salt);
            hashCode.Add(CourseIds);
            return hashCode.ToHashCode();
        }
    }
}