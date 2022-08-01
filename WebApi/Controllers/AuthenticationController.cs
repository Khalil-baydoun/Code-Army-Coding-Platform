using System.Security.Claims;
using System.Threading.Tasks;
using DataContracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public AuthenticationController(IAuthenticationService authenticationService, IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var authResult = _authenticationService.AuthenticateLogin(loginRequest.Email, loginRequest.Password);
            return Ok(authResult);
        }

        [HttpGet("GetRole")]
        [Authorize]
        public IActionResult GetRole()
        {
            var role = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role).Value;
            return Ok(new { Role = role });
        }
    }
}