using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Services.Interfaces;
using MediatR;

namespace GrubPix.Application.Features.User
{
    public class RegisterCommand : IRequest<BaseUserDto>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public RegisterCommand(string username, string email, string password, string role)
        {
            Username = username;
            Email = email;
            Password = password;
            Role = role ?? "Customer";
        }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BaseUserDto>
        {
            private readonly IMapper _mapper;
            private readonly IAuthService _authService;

            public RegisterCommandHandler(IAuthService authService, IMapper mapper)
            {
                _authService = authService;
                _mapper = mapper;
            }

            public async Task<BaseUserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var registerDto = new RegisterDto
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Role = request.Role ?? "Customer"
                };

                var baseUserDto = await _authService.RegisterAsync(registerDto);
                return baseUserDto;
            }
        }
    }
}
