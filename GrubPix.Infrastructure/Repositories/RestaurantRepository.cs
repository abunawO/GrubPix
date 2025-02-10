using Microsoft.EntityFrameworkCore;
using GrubPix.Domain.Entities;
using GrubPix.Infrastructure.Persistence;
using GrubPix.Domain.Interfaces.Repositories;

namespace GrubPix.Infrastructure.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly GrubPixDbContext _context;

        public RestaurantRepository(GrubPixDbContext context)
        {
            _context = context;
        }

        public async Task<List<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants
                .Include(r => r.Menus)
                    .ThenInclude(m => m.MenuItems) // Ensure this line exists
                .ToListAsync();
        }

        public async Task<Restaurant> GetByIdAsync(int id)
        {
            return await _context.Restaurants
                .Include(r => r.Menus)
                    .ThenInclude(m => m.MenuItems)  // Ensure MenuItems are eagerly loaded
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Restaurant> AddAsync(Restaurant restaurant)
        {
            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();
            return restaurant; // Return the newly created restaurant
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
