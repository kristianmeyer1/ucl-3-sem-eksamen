using Danplanner.Application.Dtos;
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

        public async Task<List<AdminDto>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.LoadAdminListAsync();
            return admins.Select(a => new AdminDto { AdminId = a.AdminId }).ToList();
        }
    }
}
