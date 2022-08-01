
using DataContracts.Users;

namespace DataContracts.Authentication
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Role Role { get; set; }
    }
}
