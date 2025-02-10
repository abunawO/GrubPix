using System.Collections.Generic;
using System.Linq;
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
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuService(IMenuRepository menuRepository, IMenuItemRepository menuItemRepository)
        {
            _menuRepository = menuRepository;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<IEnumerable<MenuDto>> GetMenusAsync()
        {
            var menus = await _menuRepository.GetAllAsync();
            var menuItems = await _menuItemRepository.GetAllAsync();

            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                RestaurantId = m.RestaurantId,
                Name = m.Name,
                Description = m.Description,
                Items = menuItems
                    .Where(item => item.MenuId == m.Id)
                    .Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        MenuId = item.MenuId,
                        ImageUrl = item.ImageUrl
                    })
                    .ToList()
            });
        }

        public async Task<MenuDto> GetMenuByIdAsync(int id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null) return null;

            var menuItems = await _menuItemRepository.GetAllAsync();

            return new MenuDto
            {
                Id = menu.Id,
                RestaurantId = menu.RestaurantId,
                Name = menu.Name,
                Description = menu.Description,
                Items = menuItems
                    .Where(item => item.MenuId == menu.Id)
                    .Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        MenuId = item.MenuId,
                        ImageUrl = item.ImageUrl
                    })
                    .ToList()
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
                Description = menu.Description,
                Items = new List<MenuItemDto>()
            };
        }
    }
}
