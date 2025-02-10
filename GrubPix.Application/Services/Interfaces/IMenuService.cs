using System.Collections.Generic;
using System.Threading.Tasks;
using GrubPix.Application.DTO;

namespace GrubPix.Application.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDto>> GetMenusAsync();
        Task<MenuDto> GetMenuByIdAsync(int id);
        Task<MenuDto> CreateMenuAsync(CreateMenuDto menuDto);
    }
}
