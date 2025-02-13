using GrubPix.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace GrubPix.Application.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDto>> GetAllRestaurantsAsync(string? name, string? sortBy, bool descending, int page, int pageSize);
        Task<RestaurantDto> GetRestaurantByIdAsync(int id);
        Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantDto dto, IFormFile imageFile);
        Task<RestaurantDto> UpdateRestaurantAsync(int id, CreateRestaurantDto dto, IFormFile imageFile);
        Task<bool> DeleteRestaurantAsync(int id);
    }
}
