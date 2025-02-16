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

        public async Task<List<Restaurant>> GetAllAsync(string? name, string? sortBy, bool descending, int page, int pageSize)
        {
            var query = _context.Restaurants
                .Include(r => r.Menus)
                .ThenInclude(m => m.MenuItems)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var property = typeof(Restaurant).GetProperty(sortBy);
                if (property != null)
                {
                    query = descending
                        ? query.OrderByDescending(r => EF.Property<object>(r, sortBy))
                        : query.OrderBy(r => EF.Property<object>(r, sortBy));
                }
            }

            // Ensure pageSize is always greater than 0
            if (pageSize <= 0) pageSize = 10; // Default to 10 if invalid

            // Ensure page is at least 1
            if (page < 1) page = 1;

            // Get total records count
            var totalRecords = await query.CountAsync();

            // Ensure the page is within range
            if ((page - 1) * pageSize >= totalRecords)
            {
                return new List<Restaurant>(); // Return an empty list if the page is out of range
            }

            // Apply pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
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

        public async Task<Restaurant> UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
            return restaurant; // Returning the updated restaurant
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Menus)
                .ThenInclude(m => m.MenuItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant != null)
            {
                foreach (var menu in restaurant.Menus)
                {
                    _context.MenuItems.RemoveRange(menu.MenuItems);
                }

                _context.Menus.RemoveRange(restaurant.Menus);
                _context.Restaurants.Remove(restaurant);

                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
