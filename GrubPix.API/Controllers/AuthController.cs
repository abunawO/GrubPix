using GrubPix.Application.Common;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Features.User;
using GrubPix.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GrubPix.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILogger<AuthController> logger, IMediator mediator)
        {
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
                var baseUser = await _mediator.Send(new RegisterCommand(dto.Username, dto.Email, dto.Password, dto.Role));

                if (baseUser == null)
                    return BadRequest(ApiResponse<object>.FailResponse("Registration failed"));

                _logger.LogInformation("User {Email} registered successfully", dto.Email);
                return CreatedAtAction(nameof(Register), ApiResponse<BaseUserDto>.SuccessResponse(baseUser, "User registered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", dto.Email);
                return BadRequest(ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {

                _logger.LogInformation("Login attempt for {Email}", dto.Email);
                var baseUser = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));

                if (baseUser == null)
                {
                    _logger.LogWarning("Failed login attempt for {Email}", dto.Email);
                    return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or passwordssss"));
                }

                _logger.LogInformation("User {Email} logged in successfully", dto.Email);
                return Ok(ApiResponse<BaseUserDto>.SuccessResponse(baseUser, "Login successful"));
            }
            catch (EmailNotVerifiedException ex)
            {
                _logger.LogWarning("Login attempt failed for {Email}: {Message}", dto.Email, ex.Message);
                return Unauthorized(ApiResponse<object>.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login attempt failed for {Email}", dto.Email);
                return StatusCode(500, ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyRequest request)
        {
            try
            {
                bool result = await _mediator.Send(new VerifyEmailCommand(request.Token));
                return result ? Ok(ApiResponse<object>.SuccessResponse("Email verified successfully!"))
                            : BadRequest(ApiResponse<object>.FailResponse("Verification failed."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(ex.Message));
            }
        }

    }

    public class VerifyRequest
    {
        public string Token { get; set; }
    }
}
