using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;

namespace GrubPix.Application.Features.Auth
{
    public class ResendVerificationEmailCommand : IRequest<bool>
    {
        public string Email { get; set; }

        public ResendVerificationEmailCommand(string email)
        {
            Email = email;
        }

        public class ResendVerificationEmailHandler : IRequestHandler<ResendVerificationEmailCommand, bool>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IUserRepository _userRepository;
            private readonly IEmailService _emailService;

            public ResendVerificationEmailHandler(
                ICustomerRepository customerRepository,
                IUserRepository userRepository,
                IEmailService emailService)
            {
                _customerRepository = customerRepository;
                _userRepository = userRepository;
                _emailService = emailService;
            }

            public async Task<bool> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetCustomerByEmailAsync(request.Email);
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (customer == null && user == null)
                    return false; // Email not found

                string newToken = Guid.NewGuid().ToString();

                if (customer != null)
                {
                    if (customer.IsVerified) return false; // Already verified

                    customer.VerificationToken = newToken;
                    await _customerRepository.UpdateAsync(customer);
                    await _emailService.SendVerificationEmail(customer.Email, newToken);
                }
                else if (user != null)
                {
                    if (user.IsVerified) return false; // Already verified

                    user.VerificationToken = newToken;
                    await _userRepository.UpdateAsync(user);
                    await _emailService.SendVerificationEmail(user.Email, newToken);
                }

                return true; // Successfully resent verification email
            }
        }
    }
}
