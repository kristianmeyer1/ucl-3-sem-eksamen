using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces.AdminInterfaces
{
    public interface IAdminRepository
    {
        Task<List<Admin>> LoadAdminListAsync();
        Task AddAsync(Admin admin);
        Task<Admin?> GetByIdAsync(int adminId);
    }
}
