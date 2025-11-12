using Danplanner.Application.Interfaces;
using Danplanner.Domain.Entities;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DbManager _dbManager;

        public AdminRepository(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<Admin>> LoadAdminListAsync() =>
            await _dbManager.Admin.ToListAsync();
    }

}
