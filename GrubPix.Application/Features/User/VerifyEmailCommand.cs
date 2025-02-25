using GrubPix.Application.Interfaces.Services;
using MediatR;

namespace GrubPix.Application.Features.User
{
    public class VerifyEmailCommand : IRequest<bool>
    {
        public string Token { get; }

        public VerifyEmailCommand(string token)
        {
            Token = token;
        }
    }

    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly IAuthService _authService;

        public VerifyEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.VerifyEmailAsync(request.Token);
        }
    }

}
