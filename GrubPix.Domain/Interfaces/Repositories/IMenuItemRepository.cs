using GrubPix.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Domain.Interfaces.Repositories
{
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<MenuItem> GetByIdAsync(int id);
        Task AddAsync(MenuItem menuItem);
        Task UpdateAsync(MenuItem menuItem);
        Task DeleteAsync(int id);
    }
}
