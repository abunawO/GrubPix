using GrubPix.Application.Common;
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
        Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto, ICollection<IFormFile> imageFiles);
        Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto updateMenuItemDto, ICollection<IFormFile> imageFiles);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<ApiResponse<object>> DeleteMenuItemImageAsync(int menuItemId, int imageId);
    }
}
