using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Application.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync();
        Task<MenuItemDto> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto, IFormFile imageFile);
        Task<MenuItemDto> UpdateMenuItemAsync(int id, CreateMenuItemDto updateMenuItemDto, IFormFile imageFile);
        Task<bool> DeleteMenuItemAsync(int id);
    }
}
