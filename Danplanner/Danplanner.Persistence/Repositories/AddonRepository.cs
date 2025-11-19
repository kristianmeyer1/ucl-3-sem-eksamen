using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories
{
    public class AddonRepository : IAddonRepository
    {
        private readonly DbManager _dbManager;

        public AddonRepository(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<AddonDto>> GetAllAddonsAsync()
        {
            return await _dbManager.Addon
                .Select(u => new AddonDto
                {
                    AddonId = u.AddonId,
                    AddonName = u.AddonName,
                    AddonPrice = u.AddonPrice,
                    AddonDescription = u.AddonDescription
                })
                .ToListAsync();
        }
    }
}
