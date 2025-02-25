
using GrubPix.Application.DTO;

namespace GrubPix.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseUserDto> RegisterAsync(RegisterDto dto);
        Task<BaseUserDto> AuthenticateAsync(LoginDto dto);
        Task<bool> VerifyEmailAsync(string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
