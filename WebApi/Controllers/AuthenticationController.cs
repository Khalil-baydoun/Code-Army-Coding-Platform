using System.Security.Claims;
using DataContracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
            var role = HelperFunctions.GetRole(User);
            return Ok(new { Role = role });
        }
    }
}