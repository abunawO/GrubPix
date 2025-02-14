using GrubPix.Application.DTO;
using GrubPix.Domain.Entities;

namespace GrubPix.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserDto?> AuthenticateAsync(LoginDto dto);
        Task<UserDto> RegisterAsync(RegisterDto dto);

        // Add Hashing & Verification methods
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
