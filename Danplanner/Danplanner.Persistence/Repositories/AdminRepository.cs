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

        public async Task<List<Admin>> LoadAdminListAsync()
        {
            return await _dbManager.Admin.ToListAsync();
        }

        public async Task<Admin?> GetByIdAsync(int adminId)
        {
            return await _dbManager.Admin.FirstOrDefaultAsync(a => a.AdminId == adminId);
        }


        public async Task AddAsync(Admin admin)
        {
            await _dbManager.Admin.AddAsync(admin);
            await _dbManager.SaveChangesAsync();
        }
    }
}
