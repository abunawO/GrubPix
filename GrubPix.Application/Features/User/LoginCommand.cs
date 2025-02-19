using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using MediatR;

namespace GrubPix.Application.Features.User
{
    public class LoginCommand : IRequest<UserDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, UserDto>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginDto = new LoginDto
            {
                Email = request.Email,
                Password = request.Password
            };

            var baseUserDto = await _userService.AuthenticateAsync(loginDto);
            return _mapper.Map<UserDto>(baseUserDto);

        }
    }
}
