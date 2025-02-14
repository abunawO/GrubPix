using Microsoft.AspNetCore.Mvc;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Authorization;

namespace GrubPix.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var user = await _userService.RegisterAsync(dto);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(user);
        }
    }
}
