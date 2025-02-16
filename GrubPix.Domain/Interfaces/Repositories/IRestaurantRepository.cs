using GrubPix.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Domain.Interfaces.Repositories
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetByUserIdAsync(string? name, string? sortBy, bool descending, int page, int pageSize, int userId);
        Task<Restaurant> GetByIdAsync(int id);
        Task<List<Restaurant>> GetByOwnerIdAsync(int ownerId);
        Task<Restaurant> AddAsync(Restaurant restaurant);

        Task<Restaurant> UpdateAsync(Restaurant restaurant);
        Task<bool> DeleteAsync(int id);
    }
}
