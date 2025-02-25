
using GrubPix.Application.DTO;

namespace GrubPix.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string token);
        Task SendEmailAsync(string to, string subject, string body);
    }
}
