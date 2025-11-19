using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.AddonInterfaces
{
    public interface IAddonRepository
    {
        Task<List<AddonDto>> GetAllAddonsAsync();
    }
}
