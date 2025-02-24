using GrubPix.Application.Exceptions;
using GrubPix.Application.Interfaces;
using GrubPix.Domain.Entities;
using GrubPix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrubPix.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly GrubPixDbContext _context;

        public CustomerRepository(GrubPixDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.FavoriteMenuItems)
                .ThenInclude(fm => fm.MenuItem)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task AddFavoriteAsync(int customerId, int menuItemId)
        {
            var favorite = new FavoriteMenuItem
            {
                CustomerId = customerId,
                MenuItemId = menuItemId
            };
            _context.FavoriteMenuItems.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(int customerId, int menuItemId)
        {
            var favorite = await _context.FavoriteMenuItems
                .FirstOrDefaultAsync(f => f.CustomerId == customerId && f.MenuItemId == menuItemId);

            if (favorite != null)
            {
                _context.FavoriteMenuItems.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MenuItem>> GetFavoriteMenuItemsAsync(int customerId)
        {
            return await _context.FavoriteMenuItems
                .Where(f => f.CustomerId == customerId)
                .Select(f => new MenuItem
                {
                    Id = f.MenuItem.Id,
                    Name = f.MenuItem.Name,
                    Description = f.MenuItem.Description,
                    Price = f.MenuItem.Price,
                    Images = f.MenuItem.Images.Select(img => new MenuItemImage
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.Id);
            if (existingCustomer == null) throw new NotFoundException($"Customer with ID {customer.Id} not found.");

            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer> GetCustomerByUsernameAsync(string username)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
        }

    }
}
