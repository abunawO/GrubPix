using GrubPix.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrubPix.Domain.Interfaces.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<Menu> GetByIdAsync(int id);
        Task<Menu> AddAsync(Menu menu);
        Task UpdateAsync(Menu menu);
        Task DeleteAsync(int id);
    }
}
