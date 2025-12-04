using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models.ModelsDto;
using Danplanner.Persistence.DbMangagerDir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories.AddonRepositories
{
    public class AddonRepositoryGet : IAddonGetAll/*, IAddonGetByBookingId*/
    {
        private readonly DbManager _dbManager;

        public AddonRepositoryGet(DbManager dbManager)
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

        //public async Task<List<Addon>> AddonGetByBookingIdAsync(int id)
        //{
        //    return await _dbManager.Addon
        //        .Where(a => a.BookingId == id)
        //        .Select(a => new AddonDto 
        //        { 
        //            AddonId = a.AddonId, 
        //            AddonName = a.AddonName, 
        //            AddonDescription = a.AddonDescription 
        //        });
        //}
    }
}
