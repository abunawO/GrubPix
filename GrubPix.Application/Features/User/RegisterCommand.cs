using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using MediatR;

namespace GrubPix.Application.Features.User
{
    public class RegisterCommand : IRequest<UserDto>
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

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public RegisterCommandHandler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var registerDto = new RegisterDto
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Role = request.Role ?? "Customer"
                };

                var baseUserDto = await _userService.RegisterAsync(registerDto);
                return _mapper.Map<UserDto>(baseUserDto);
            }
        }
    }
}
