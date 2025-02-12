using GrubPix.Application.DTO;
using GrubPix.Application.Exceptions;
using GrubPix.Application.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<RestaurantService> _logger;

    public RestaurantService(IRestaurantRepository restaurantRepository, IImageStorageService imageStorageService, IMenuRepository menuRepository, IMenuItemRepository menuItemRepository, ILogger<RestaurantService> logger)
    {
        _restaurantRepository = restaurantRepository;
        _imageStorageService = imageStorageService;
        _menuRepository = menuRepository;
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RestaurantDto>> GetAllRestaurantsAsync()
    {
        var restaurants = await _restaurantRepository.GetAllAsync();

        return restaurants.Select(r => new RestaurantDto
        {
            Id = r.Id,
            Name = r.Name,
            Address = r.Address,
            ImageUrl = r.ImageUrl,
            Menus = r.Menus.Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                RestaurantId = m.RestaurantId,
                Items = m.MenuItems.Select(item =>
                {
                    return new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        MenuId = item.MenuId,
                        ImageUrl = item.ImageUrl
                    };
                }).ToList()
            }).ToList()
        }).ToList();
    }


    public async Task<RestaurantDto> GetRestaurantByIdAsync(int id)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id);
        if (restaurant == null) throw new NotFoundException($"Restaurant with ID {id} not found.");

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



    public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantDto restaurantDto, IFormFile imageFile)
    {
        try
        {
            var restaurant = new Restaurant
            {
                Name = restaurantDto.Name,
                Address = restaurantDto.Address,
                Description = restaurantDto.Description
            };

            if (imageFile != null)
            {
                var imageUrl = await _imageStorageService.UploadImageAsync(imageFile.OpenReadStream());
                restaurant.ImageUrl = imageUrl;
            }

            await _restaurantRepository.AddAsync(restaurant);

            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                ImageUrl = restaurant.ImageUrl,
                Description = restaurant.Description

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the restaurant.");
            throw new InternalServerErrorException("An error occurred while creating the restaurant. Please try again later.");
        }

    }

    public async Task<RestaurantDto> UpdateRestaurantAsync(int id, CreateRestaurantDto restaurantDto, IFormFile imageFile)
    {
        var existingRestaurant = await _restaurantRepository.GetByIdAsync(id);
        if (existingRestaurant == null) throw new NotFoundException($"Restaurant with ID {id} not found.");

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

    public async Task<bool> DeleteRestaurantAsync(int id)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id);
        if (restaurant == null) throw new NotFoundException($"Restaurant with ID {id} not found.");

        // Collect menus and menu items before deletion
        var menusToDelete = restaurant.Menus.ToList();
        foreach (var menu in menusToDelete)
        {
            var menuItemsToDelete = menu.MenuItems.ToList();
            foreach (var menuItem in menuItemsToDelete)
            {
                await _menuItemRepository.DeleteAsync(menuItem.Id);
            }
            await _menuRepository.DeleteAsync(menu.Id);
        }

        // Delete restaurant image if exists
        if (!string.IsNullOrEmpty(restaurant.ImageUrl))
        {
            await _imageStorageService.DeleteImageAsync(restaurant.ImageUrl);
        }

        // Delete restaurant
        await _restaurantRepository.DeleteAsync(restaurant.Id);
        return true;
    }

}
