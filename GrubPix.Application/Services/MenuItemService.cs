using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Services
{
    /// <summary>
    /// Service for managing menu items.
    /// </summary>
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<MenuItemService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemService"/> class.
        /// </summary>
        public MenuItemService(
            IMenuItemRepository menuItemRepository,
            IImageStorageService imageStorageService,
            ILogger<MenuItemService> logger)
        {
            _menuItemRepository = menuItemRepository;
            _imageStorageService = imageStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all menu items from the repository.
        /// </summary>
        /// <returns>A collection of <see cref="MenuItemDto"/> objects representing all menu items.</returns>
        public async Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _menuItemRepository.GetAllAsync();
            return menuItems.Select(item => new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                MenuId = item.MenuId,
                ImageUrl = item.ImageUrl
            }).ToList();
        }

        /// <summary>
        /// Retrieves a specific menu item by its ID.
        /// </summary>
        /// <param name="id">The ID of the menu item to retrieve.</param>
        /// <returns>A <see cref="MenuItemDto"/> object representing the menu item.</returns>
        /// <exception cref="NotFoundException">Thrown if the menu item with the specified ID is not found.</exception>
        public async Task<MenuItemDto> GetMenuItemByIdAsync(int id)
        {
            var item = await _menuItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Menu item with ID {Id} not found.", id);
                throw new NotFoundException($"Menu item with ID {id} not found.");
            }

            return new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                MenuId = item.MenuId,
                ImageUrl = item.ImageUrl
            };
        }

        /// <summary>
        /// Creates a new menu item and stores it in the repository.
        /// </summary>
        /// <param name="createMenuItemDto">The data transfer object containing the details of the new menu item.</param>
        /// <param name="imageFile">The image file associated with the menu item.</param>
        /// <returns>A <see cref="MenuItemDto"/> object representing the newly created menu item.</returns>
        /// <exception cref="InternalServerErrorException">Thrown if an error occurs while creating the menu item.</exception>
        public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto, IFormFile imageFile)
        {
            try
            {
                var newItem = new MenuItem
                {
                    Name = createMenuItemDto.Name,
                    Description = createMenuItemDto.Description,
                    Price = createMenuItemDto.Price,
                    MenuId = createMenuItemDto.MenuId
                };

                if (imageFile != null)
                {
                    using var stream = imageFile.OpenReadStream();
                    newItem.ImageUrl = await _imageStorageService.UploadImageAsync(stream);
                }

                await _menuItemRepository.AddAsync(newItem);
                return await GetMenuItemByIdAsync(newItem.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the menu item.");
                throw new InternalServerErrorException("An error occurred while creating the menu item. Please try again later.");
            }
        }

        /// <summary>
        /// Updates an existing menu item with new details.
        /// </summary>
        /// <param name="id">The ID of the menu item to update.</param>
        /// <param name="updateMenuItemDto">The data transfer object containing the updated details of the menu item.</param>
        /// <param name="imageFile">The new image file associated with the menu item (optional).</param>
        /// <returns>A <see cref="MenuItemDto"/> object representing the updated menu item.</returns>
        /// <exception cref="NotFoundException">Thrown if the menu item with the specified ID is not found.</exception>
        public async Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto updateMenuItemDto, IFormFile imageFile)
        {
            var existingItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning("Menu item with ID {Id} not found for update.", id);
                throw new NotFoundException($"Menu item with ID {id} not found.");
            }

            existingItem.Name = updateMenuItemDto.Name;
            existingItem.Description = updateMenuItemDto.Description;
            existingItem.Price = updateMenuItemDto.Price;

            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                existingItem.ImageUrl = await _imageStorageService.UploadImageAsync(stream);
            }
            else
            {
                existingItem.ImageUrl = updateMenuItemDto.ImageUrl;
            }

            await _menuItemRepository.UpdateAsync(existingItem);
            return await GetMenuItemByIdAsync(id);
        }

        /// <summary>
        /// Deletes a menu item by its ID.
        /// </summary>
        /// <param name="id">The ID of the menu item to delete.</param>
        /// <returns>True if the menu item was successfully deleted.</returns>
        /// <exception cref="NotFoundException">Thrown if the menu item with the specified ID is not found.</exception>
        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var item = await _menuItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Attempted to delete non-existent menu item with ID {Id}.", id);
                throw new NotFoundException($"Menu item with ID {id} not found.");
            }

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                await _imageStorageService.DeleteImageAsync(item.ImageUrl);
            }

            await _menuItemRepository.DeleteAsync(item.Id);
            return true;
        }
    }
}
