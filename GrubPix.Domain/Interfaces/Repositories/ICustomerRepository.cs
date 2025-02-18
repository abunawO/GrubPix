using GrubPix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task AddFavoriteAsync(int customerId, int menuItemId);
        Task RemoveFavoriteAsync(int customerId, int menuItemId);
        Task<List<MenuItem>> GetFavoriteMenuItemsAsync(int customerId);

        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
        Task<Customer> GetCustomerByUsernameAsync(string username);
    }
}
