using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Application.Services;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrubPix.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace GrubPix.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<MenuItemService> _logger;

        public MenuItemService(IMenuItemRepository menuItemRepository, IImageStorageService imageStorageService, ILogger<MenuItemService> logger)
        {
            _menuItemRepository = menuItemRepository;
            _imageStorageService = imageStorageService;
            _logger = logger;
        }

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

        public async Task<MenuItemDto> GetMenuItemByIdAsync(int id)
        {
            var item = await _menuItemRepository.GetByIdAsync(id);
            if (item == null) throw new NotFoundException($"MenuItem with ID {id} not found.");

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
                    using (var stream = imageFile.OpenReadStream())
                    {
                        var imageUrl = await _imageStorageService.UploadImageAsync(stream);
                        newItem.ImageUrl = imageUrl;
                    }
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


        public async Task<MenuItemDto> UpdateMenuItemAsync(int id, CreateMenuItemDto updateMenuItemDto, IFormFile imageFile)
        {
            var existingItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingItem == null) throw new NotFoundException($"MenuItem with ID {id} not found.");

            existingItem.Name = updateMenuItemDto.Name;
            existingItem.Description = updateMenuItemDto.Description;
            existingItem.Price = updateMenuItemDto.Price;

            if (imageFile != null)
            {
                using (var stream = imageFile.OpenReadStream())
                {
                    var imageUrl = await _imageStorageService.UploadImageAsync(stream);
                    existingItem.ImageUrl = imageUrl;
                }
            }
            else
            {
                existingItem.ImageUrl = updateMenuItemDto.ImageUrl;
            }

            await _menuItemRepository.UpdateAsync(existingItem);
            return await GetMenuItemByIdAsync(id);
        }


        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var item = await _menuItemRepository.GetByIdAsync(id);
            if (item == null) throw new NotFoundException($"MenuItem with ID {id} not found.");

            // Delete the image from S3 if it exists
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                await _imageStorageService.DeleteImageAsync(item.ImageUrl);
            }

            await _menuItemRepository.DeleteAsync(item.Id);
            return true;
        }

    }
}
