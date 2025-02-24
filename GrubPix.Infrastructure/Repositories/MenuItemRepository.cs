using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Domain.Entities;
using GrubPix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; // If using EF Core

public class MenuItemRepository : IMenuItemRepository
{
    private readonly GrubPixDbContext _context;

    public MenuItemRepository(GrubPixDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync()
    {
        return await _context.MenuItems
            .Include(m => m.Images) // Ensure images are included
            .ToListAsync();
    }

    public async Task<MenuItem> GetByIdAsync(int id)
    {
        return await _context.MenuItems
            .Include(m => m.Images) // Ensure images are included
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddAsync(MenuItem menuItem)
    {
        await _context.MenuItems.AddAsync(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MenuItem menuItem)
    {
        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menuItem = await GetByIdAsync(id);
        if (menuItem != null)
        {
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
        }
    }
}
