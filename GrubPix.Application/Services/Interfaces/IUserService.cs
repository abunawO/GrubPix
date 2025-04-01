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
        Task<BaseUserDto?> GetUserByIdAsync(int id);

    }
}
