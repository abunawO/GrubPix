using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace GrubPix.Application.Features.Auth
{
    public class ForgotPasswordCommand : IRequest<bool>
    {
        public string Email { get; }

        public ForgotPasswordCommand(string email)
        {
            Email = email;
        }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IAuthService _authService;

        public ForgotPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ForgotPasswordAsync(request.Email);
        }
    }
}
