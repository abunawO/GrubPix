using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrubPix.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IUserRepository _userRepository;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IImageStorageService imageStorageService,
            IMenuRepository menuRepository,
            IMenuItemRepository menuItemRepository,
            ILogger<RestaurantService> logger, IUserRepository userRepository)
        {
            _restaurantRepository = restaurantRepository;
            _imageStorageService = imageStorageService;
            _menuRepository = menuRepository;
            _menuItemRepository = menuItemRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get all restaurants with optional sorting and pagination.
        /// </summary>
        public async Task<IEnumerable<RestaurantDto>> GetRestaurantsByUserIdAsync(string? name, string? sortBy, bool descending, int page, int pageSize, int userId)
        {
            var restaurants = await _restaurantRepository.GetByUserIdAsync(name, sortBy, descending, page, pageSize, userId);

            return restaurants.Select(r => new RestaurantDto
            {
                OwnerId = userId,
                Id = r.Id,
                Name = r.Name,
                Address = r.Address,
                ImageUrl = r.ImageUrl,
                Description = r.Description,
                Menus = r.Menus.Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    RestaurantId = m.RestaurantId,
                    Items = m.MenuItems.Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        MenuId = item.MenuId,
                        ImageUrl = item.ImageUrl
                    }).ToList()
                }).ToList()
            });
        }

        /// <summary>
        /// Get a single restaurant by ID.
        /// </summary>
        public async Task<RestaurantDto> GetRestaurantByIdAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
            {
                _logger.LogWarning("Restaurant with ID {Id} not found.", id);
                throw new NotFoundException($"Restaurant with ID {id} not found.");
            }

            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                ImageUrl = restaurant.ImageUrl,
                Description = restaurant.Description,
                Menus = restaurant.Menus.Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    RestaurantId = m.RestaurantId,
                    Items = m.MenuItems.Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        MenuId = item.MenuId,
                        ImageUrl = item.ImageUrl
                    }).ToList()
                }).ToList()
            };
        }

        /// <summary>
        /// Create a new restaurant.
        /// </summary>
        public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantDto restaurantDto, IFormFile? imageFile)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(restaurantDto.OwnerId);

                if (user == null)
                    throw new NotFoundException("User not found");

                var restaurant = new Restaurant
                {
                    Name = restaurantDto.Name,
                    Address = restaurantDto.Address,
                    Description = restaurantDto.Description,
                    OwnerId = restaurantDto.OwnerId
                };

                if (imageFile != null)
                {
                    restaurant.ImageUrl = await _imageStorageService.UploadImageAsync(imageFile.OpenReadStream());
                }

                await _restaurantRepository.AddAsync(restaurant);
                _logger.LogInformation("Restaurant {RestaurantName} created successfully", restaurant.Name);

                return new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    ImageUrl = restaurant.ImageUrl,
                    Description = restaurant.Description,
                    OwnerId = restaurant.OwnerId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the restaurant.");
                throw new InternalServerErrorException("An error occurred while creating the restaurant. Please try again later.");
            }
        }

        /// <summary>
        /// Update restaurant details.
        /// </summary>
        public async Task<RestaurantDto> UpdateRestaurantAsync(int id, UpdateRestaurantDto restaurantDto, IFormFile? imageFile)
        {
            var existingRestaurant = await _restaurantRepository.GetByIdAsync(id);
            if (existingRestaurant == null)
            {
                _logger.LogWarning("Restaurant with ID {Id} not found.", id);
                throw new NotFoundException($"Restaurant with ID {id} not found.");
            }

            existingRestaurant.Name = restaurantDto.Name;
            existingRestaurant.Address = restaurantDto.Address;
            existingRestaurant.Description = restaurantDto.Description;

            if (imageFile != null)
            {
                var imageUrl = await _imageStorageService.UploadImageAsync(imageFile.OpenReadStream());
                existingRestaurant.ImageUrl = imageUrl;
            }

            await _restaurantRepository.UpdateAsync(existingRestaurant);

            return new RestaurantDto
            {
                Id = existingRestaurant.Id,
                Name = existingRestaurant.Name,
                Address = existingRestaurant.Address,
                ImageUrl = existingRestaurant.ImageUrl,
                Description = existingRestaurant.Description
            };
        }

        /// <summary>
        /// Delete a restaurant.
        /// </summary>
        public async Task<bool> DeleteRestaurantAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
            {
                _logger.LogWarning("Restaurant with ID {Id} not found.", id);
                throw new NotFoundException($"Restaurant with ID {id} not found.");
            }

            foreach (var menu in restaurant.Menus)
            {
                foreach (var menuItem in menu.MenuItems)
                {
                    await _menuItemRepository.DeleteAsync(menuItem.Id);
                }
                await _menuRepository.DeleteAsync(menu.Id);
            }

            if (!string.IsNullOrEmpty(restaurant.ImageUrl))
            {
                await _imageStorageService.DeleteImageAsync(restaurant.ImageUrl);
            }

            await _restaurantRepository.DeleteAsync(restaurant.Id);
            _logger.LogInformation("Restaurant {RestaurantName} deleted successfully", restaurant.Name);
            return true;
        }
    }
}
