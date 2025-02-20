using GrubPix.Application.Common;
using GrubPix.Application.DTO;
using GrubPix.Application.Features.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var user = await _mediator.Send(new RegisterCommand(dto.Username, dto.Email, dto.Password, dto.Role));

            if (user == null)
                return BadRequest(ApiResponse<object>.FailResponse("Registration failed"));

            _logger.LogInformation("User {Email} registered successfully", dto.Email);
            return CreatedAtAction(nameof(Register), ApiResponse<UserDto>.SuccessResponse(user, "User registered successfully"));
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
        _logger.LogInformation("Login attempt for {Email}", dto.Email);
        var user = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));

        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for {Email}", dto.Email);
            return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password"));
        }

        _logger.LogInformation("User {Email} logged in successfully", dto.Email);
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Login successful"));
    }
}
