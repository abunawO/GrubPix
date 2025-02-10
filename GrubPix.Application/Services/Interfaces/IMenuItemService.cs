using GrubPix.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Application.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync();
        Task<MenuItemDto> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto);
        Task<MenuItemDto> UpdateMenuItemAsync(int id, CreateMenuItemDto updateMenuItemDto);
        Task<bool> DeleteMenuItemAsync(int id);
    }
}
