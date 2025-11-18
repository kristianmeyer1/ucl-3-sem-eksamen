using Danplanner.Domain.Entities;
using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.AdminInterfaces
{
    public interface IAdminRepository
    {
        Task<List<Admin>> LoadAdminListAsync();
        Task AddAsync(Admin admin);
        Task<Admin?> GetByIdAsync(int adminId);
    }
}
