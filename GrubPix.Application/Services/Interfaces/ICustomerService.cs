using System.Collections.Generic;
using System.Threading.Tasks;
using GrubPix.Application.DTO;
using GrubPix.Application.DTOs;

namespace GrubPix.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateCustomerAsync(BaseUserDto dto);
        Task<CustomerDto> GetCustomerByEmailAsync(string email);
        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<CustomerDto> GetCustomerByUsernameAsync(string username);
        Task<bool> AddFavoriteAsync(int customerId, int menuItemId);
        Task<bool> RemoveFavoriteAsync(int customerId, int menuItemId);
        Task<List<MenuItemDto>> GetFavoriteMenuItemsAsync(int customerId);
        Task<bool> UpdateAsync(int id, UpdateCustomerDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
