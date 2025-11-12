using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<Admin>> LoadAdminListAsync();
    }
}
