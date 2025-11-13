using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<Admin>> LoadAdminListAsync();
        Task AddAsync(Admin admin);
        Task<Admin?> GetByIdAsync(int adminId);
    }
}
