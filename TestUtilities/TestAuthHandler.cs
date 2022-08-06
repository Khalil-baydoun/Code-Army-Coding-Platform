using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TestUtilities
{
    public class TestAuthHandlerOptions : AuthenticationSchemeOptions
    {
    }

    public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
    {
        public const string AuthenticationScheme = "Test";
        public const string Role = "Role";
        public const string Email = "Email";

        public TestAuthHandler(
            IOptionsMonitor<TestAuthHandlerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>();

            if (Context.Request.Headers.TryGetValue(Role, out var role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            if (Context.Request.Headers.TryGetValue(Email, out var email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Email, "testuser@codearmy.com"));
            }


            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
