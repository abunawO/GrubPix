using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace GrubPix.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ILogger<MenuService> _logger;
        private readonly IMapper _mapper;

        public MenuService(IMenuRepository menuRepository, IMenuItemRepository menuItemRepository, ILogger<MenuService> logger, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _menuItemRepository = menuItemRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all menus along with their associated menu items.
        /// </summary>
        /// <returns>A collection of menus with menu items.</returns>
        public async Task<IEnumerable<MenuDto>> GetMenusAsync()
        {
            var menus = await _menuRepository.GetAllAsync();
            //var menuItems = await _menuItemRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }

        /// <summary>
        /// Retrieves a menu by its ID along with its associated menu items.
        /// </summary>
        /// <param name="id">The ID of the menu.</param>
        /// <returns>The requested menu details.</returns>
        /// <exception cref="NotFoundException">Thrown when the menu is not found.</exception>
        public async Task<MenuDto> GetMenuByIdAsync(int id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
                throw new NotFoundException($"Menu with ID {id} not found.");

            var menuItems = await _menuItemRepository.GetAllAsync();

            return _mapper.Map<MenuDto>(menu);
        }

        /// <summary>
        /// Creates a new menu.
        /// </summary>
        /// <param name="menuDto">The details of the menu to be created.</param>
        /// <returns>The created menu.</returns>
        /// <exception cref="InternalServerErrorException">Thrown when menu creation fails.</exception>
        public async Task<MenuDto> CreateMenuAsync(CreateMenuDto menuDto)
        {
            try
            {
                var menu = new Menu
                {
                    RestaurantId = menuDto.RestaurantId,
                    Name = menuDto.Name,
                    Description = menuDto.Description
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the menu.");
                throw new InternalServerErrorException(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing menu.
        /// </summary>
        /// <param name="id">The ID of the menu to update.</param>
        /// <param name="menuDto">The updated menu details.</param>
        /// <returns>The updated menu.</returns>
        /// <exception cref="NotFoundException">Thrown when the menu is not found.</exception>
        public async Task<MenuDto> UpdateMenuAsync(int id, UpdateMenuDto menuDto)
        {
            var existingMenu = await _menuRepository.GetByIdAsync(id);
            if (existingMenu == null)
                throw new NotFoundException($"Menu with ID {id} not found.");

            existingMenu.Name = menuDto.Name;
            existingMenu.Description = menuDto.Description;

            await _menuRepository.UpdateAsync(existingMenu);

            return _mapper.Map<MenuDto>(existingMenu);
        }

        /// <summary>
        /// Deletes a menu and its associated menu items.
        /// </summary>
        /// <param name="id">The ID of the menu to delete.</param>
        /// <returns>True if deletion was successful.</returns>
        /// <exception cref="NotFoundException">Thrown when the menu is not found.</exception>
        public async Task<bool> DeleteMenuAsync(int id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
                throw new NotFoundException($"Menu with ID {id} not found.");

            // Delete associated menu items
            foreach (var menuItem in menu.MenuItems.ToList())
            {
                await _menuItemRepository.DeleteAsync(menuItem.Id);
            }

            await _menuRepository.DeleteAsync(menu.Id);
            return true;
        }
    }
}
