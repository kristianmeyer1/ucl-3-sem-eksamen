using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Persistence.Repositories
{
    public class AddonRepository : IAddonGetAll
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
