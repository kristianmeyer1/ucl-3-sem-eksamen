using Danplanner.Domain.Entities;
using Danplanner.Application.Interfaces;

namespace Danplanner.Application.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<Admin>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.LoadAdminListAsync();
            return admins.Select(a => new Admin { AdminId = a.AdminId }).ToList();
        }
    }
}
