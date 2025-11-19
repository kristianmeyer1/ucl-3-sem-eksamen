using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.AddonInterfaces
{
    public interface IAddonGetAll
    {
        Task<List<AddonDto>> GetAllAddonsAsync();
    }
}
