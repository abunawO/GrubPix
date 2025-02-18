using GrubPix.Application.DTO;
using GrubPix.Domain.Entities;

namespace GrubPix.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseUserDto?> GetByUsernameAsync(string username);
        Task<BaseUserDto?> GetByEmailAsync(string email);

        Task<BaseUserDto> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<BaseUserDto?> AuthenticateAsync(LoginDto dto);
        Task<BaseUserDto> RegisterAsync(RegisterDto dto);

        // Add Hashing & Verification methods
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
