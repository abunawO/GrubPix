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
using AutoMapper;

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
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemService"/> class.
        /// </summary>
        public MenuItemService(
            IMenuItemRepository menuItemRepository,
            IImageStorageService imageStorageService,
            ILogger<MenuItemService> logger,
            IMapper mapper)
        {
            _menuItemRepository = menuItemRepository;
            _imageStorageService = imageStorageService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all menu items from the repository.
        /// </summary>
        /// <returns>A collection of <see cref="MenuItemDto"/> objects representing all menu items.</returns>
        public async Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _menuItemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MenuItemDto>>(menuItems);
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

            return _mapper.Map<MenuItemDto>(item);
        }

        /// <summary>
        /// Creates a new menu item and stores it in the repository.
        /// </summary>
        /// <param name="createMenuItemDto">The data transfer object containing the details of the new menu item.</param>
        /// <param name="imageFile">The image file associated with the menu item.</param>
        /// <returns>A <see cref="MenuItemDto"/> object representing the newly created menu item.</returns>
        /// <exception cref="InternalServerErrorException">Thrown if an error occurs while creating the menu item.</exception>
        public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto, ICollection<IFormFile> imageFiles)
        {
            try
            {
                var newItem = new MenuItem
                {
                    Name = createMenuItemDto.Name,
                    Description = createMenuItemDto.Description,
                    Price = createMenuItemDto.Price,
                    MenuId = createMenuItemDto.MenuId,
                    Images = new List<MenuItemImage>()
                };

                // Process multiple images
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    foreach (var file in imageFiles)
                    {
                        using var stream = file.OpenReadStream();
                        var imageUrl = await _imageStorageService.UploadImageAsync(stream);

                        newItem.Images.Add(new MenuItemImage
                        {
                            ImageUrl = imageUrl
                        });
                    }
                }

                await _menuItemRepository.AddAsync(newItem);
                return await GetMenuItemByIdAsync(newItem.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the menu item.");
                throw new InternalServerErrorException(ex.Message);
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
        public async Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto updateMenuItemDto, ICollection<IFormFile> imageFiles)
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

            // Process multiple images
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    using var stream = file.OpenReadStream();
                    var imageUrl = await _imageStorageService.UploadImageAsync(stream);

                    existingItem.Images.Add(new MenuItemImage
                    {
                        ImageUrl = imageUrl
                    });
                }
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

            // Delete all associated images from storage
            if (item.Images != null && item.Images.Any())
            {
                foreach (var image in item.Images)
                {
                    if (!string.IsNullOrEmpty(image.ImageUrl))
                    {
                        try
                        {
                            await _imageStorageService.DeleteImageAsync(image.ImageUrl);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deleting image {ImageUrl} for menu item {Id}.", image.ImageUrl, id);
                        }
                    }
                }
            }

            await _menuItemRepository.DeleteAsync(item.Id);
            return true;
        }
    }
}
