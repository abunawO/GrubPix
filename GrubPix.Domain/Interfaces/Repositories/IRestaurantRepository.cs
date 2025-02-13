using GrubPix.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Domain.Interfaces.Repositories
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetAllAsync(string? name, string? sortBy, bool descending, int page, int pageSize);
        Task<Restaurant> GetByIdAsync(int id);
        Task<Restaurant> AddAsync(Restaurant restaurant);

        Task UpdateAsync(Restaurant restaurant);
        Task DeleteAsync(int id);
    }
}
