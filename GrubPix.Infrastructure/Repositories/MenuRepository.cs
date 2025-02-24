using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces;  // Correct Interface reference
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GrubPix.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly GrubPixDbContext _context;

        public MenuRepository(GrubPixDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menus
                .Include(m => m.MenuItems)
                .ThenInclude(mi => mi.Images)
                .ToListAsync();
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus
                .Include(m => m.MenuItems)
                .ThenInclude(mi => mi.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Menu> AddAsync(Menu menu)
        {
            await _context.Menus.AddAsync(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task UpdateAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menu = await _context.Menus.Include(m => m.MenuItems).FirstOrDefaultAsync(m => m.Id == id);
            if (menu != null)
            {
                // Remove associated menu items first
                _context.MenuItems.RemoveRange(menu.MenuItems);

                // Remove the menu
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }

    }
}
