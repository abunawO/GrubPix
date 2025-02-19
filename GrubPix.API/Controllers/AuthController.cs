using Microsoft.AspNetCore.Mvc;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using GrubPix.Application.Features.User;

namespace GrubPix.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(IUserService userService, ILogger<AuthController> logger, IMediator mediator)
        {
            _userService = userService;
            _logger = logger;
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Register attempt for {Email}", dto.Email);
                var user = await _mediator.Send(new RegisterCommand(dto.Username, dto.Email, dto.Password));
                _logger.LogInformation("User {Email} registered successfully", dto.Email);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", dto.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for {Email}", dto.Email);
            var user = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for {Email}", dto.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            _logger.LogInformation("User {Email} logged in successfully", dto.Email);
            return Ok(user);
        }
    }
}
