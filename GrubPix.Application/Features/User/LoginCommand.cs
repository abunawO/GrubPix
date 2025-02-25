using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Services.Interfaces;
using MediatR;

namespace GrubPix.Application.Features.User
{
    public class LoginCommand : IRequest<BaseUserDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseUserDto>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<BaseUserDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginDto = new LoginDto
            {
                Email = request.Email,
                Password = request.Password
            };

            var baseUserDto = await _authService.AuthenticateAsync(loginDto);
            return baseUserDto;

        }
    }
}
