using System.Collections.Generic;
using System.Threading.Tasks;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;

namespace GrubPix.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuDto>> GetMenusAsync()
        {
            var menus = await _menuRepository.GetAllAsync();
            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                RestaurantId = m.RestaurantId,
                Name = m.Name,
                Description = m.Description
            });
        }

        public async Task<MenuDto> GetMenuByIdAsync(int id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null) return null;

            return new MenuDto
            {
                Id = menu.Id,
                RestaurantId = menu.RestaurantId,
                Name = menu.Name,
                Description = menu.Description
            };
        }

        public async Task<MenuDto> CreateMenuAsync(CreateMenuDto menuDto)
        {
            var menu = new Menu
            {
                RestaurantId = menuDto.RestaurantId,
                Name = menuDto.Name,
                Description = (string)menuDto.Description
            };

            await _menuRepository.AddAsync(menu);

            return new MenuDto
            {
                Id = menu.Id,
                RestaurantId = menu.RestaurantId,
                Name = menu.Name,
                Description = menu.Description
            };
        }
    }
}
