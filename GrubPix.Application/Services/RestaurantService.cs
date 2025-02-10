using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;

    public RestaurantService(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
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



    public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantDto restaurantDto)
    {
        var restaurant = new Restaurant
        {
            Name = restaurantDto.Name,
            Address = restaurantDto.Address,
            ImageUrl = restaurantDto.ImageUrl // Add ImageUrl during creation
        };

        await _restaurantRepository.AddAsync(restaurant);

        // Assign the ID after creation and include ImageUrl
        return new RestaurantDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Address = restaurant.Address,
            ImageUrl = restaurant.ImageUrl // Return ImageUrl in the response
        };
    }

}
