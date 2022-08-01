using Microsoft.AspNetCore.Authorization;

namespace webapi
{
    public class Policies
    {
        public static AuthorizationPolicy AdminsAndInstructorsPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(new[] { "Admin", "Instructor" }).Build();
        }

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole("Admin").Build();
        }
    }
}