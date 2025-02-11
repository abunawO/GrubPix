using GrubPix.Application.DTO;
using GrubPix.Application.Services;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IImageStorageService _imageStorageService;

    public RestaurantService(IRestaurantRepository restaurantRepository, IImageStorageService imageStorageService)
    {
        _restaurantRepository = restaurantRepository;
        _imageStorageService = imageStorageService;
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
        if (restaurant == null) return null;

        return new RestaurantDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Address = restaurant.Address,
            ImageUrl = restaurant.ImageUrl,
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
        var restaurant = new Restaurant
        {
            Name = restaurantDto.Name,
            Address = restaurantDto.Address
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
            ImageUrl = restaurant.ImageUrl
        };
    }

    public async Task<RestaurantDto> UpdateRestaurantAsync(int id, CreateRestaurantDto restaurantDto, IFormFile imageFile)
    {
        var existingRestaurant = await _restaurantRepository.GetByIdAsync(id);
        if (existingRestaurant == null) return null;

        existingRestaurant.Name = restaurantDto.Name;
        existingRestaurant.Address = restaurantDto.Address;

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
            ImageUrl = existingRestaurant.ImageUrl
        };
    }

    public async Task<bool> DeleteRestaurantAsync(int id)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id);
        if (restaurant == null) return false;

        if (!string.IsNullOrEmpty(restaurant.ImageUrl))
        {
            await _imageStorageService.DeleteImageAsync(restaurant.ImageUrl);
        }

        await _restaurantRepository.DeleteAsync(restaurant.Id);
        return true;
    }

}
