using System.Security.Claims;
using AutoMapper;
using DataContracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest userReq)
        {
            await _userService.AddUser(userReq);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email);
            User user = _userService.GetUser(claim.Value);
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, GetUserResponse>();
            });
            var _mapper = configuration.CreateMapper();
            return Ok(_mapper.Map<GetUserResponse>(user));
        }

        [HttpGet("{email}")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUser(string email)
        {
            User user = _userService.GetUser(email);
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, GetUserResponse>();
            });
            var _mapper = configuration.CreateMapper();
            return Ok(_mapper.Map<GetUserResponse>(user));
        }

        [HttpDelete("{email}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(string email)
        {
            await _userService.DeleteUser(email);
            return Ok();
        }

        [HttpPut("{userEmail}")]
        [Authorize]
        public async Task<IActionResult> Update(string userEmail, [FromBody] AddUserRequest userReq)
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email);
            if (User.IsInRole("Admin") || userEmail.Equals(claim.Value))
            {
                await _userService.UpdateUser(userReq);
                return Ok();
            }
            return Forbid();
        }
    }
}

