using GrubPix.Application.Interfaces.Services;
using MediatR;

namespace GrubPix.Application.Features.Auth
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public string Token { get; }
        public string NewPassword { get; }

        public ResetPasswordCommand(string token, string newPassword)
        {
            Token = token;
            NewPassword = newPassword;
        }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
        }
    }
}
