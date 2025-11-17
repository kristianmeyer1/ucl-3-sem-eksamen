using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Application.Interfaces;
using Danplanner.Application.Interfaces.AdminInterfaces;
using Danplanner.Application.Models;
using Danplanner.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Danplanner.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly PasswordHasher<Admin> _passwordHasher;

        public AuthService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
            _passwordHasher = new PasswordHasher<Admin>();
        }

        public async Task<AdminDto> RegisterAsync(AdminDto request)
        {
            var existingAdmin = await _adminRepository.GetByIdAsync(request.AdminId);
            if (existingAdmin != null)
            {
                return null;            
            }

            var admin = new Admin
            {
                AdminId = request.AdminId,
                AdminPassword = _passwordHasher.HashPassword(null, request.AdminDtoPassword)
            };
            await _adminRepository.AddAsync(admin);

            return new AdminDto
            {
                AdminId = admin.AdminId,
                AdminDtoPassword = null
            };
        }
        public async Task<AdminDto?> LoginAsync(AdminDto request)
        {
            var admin = await _adminRepository.GetByIdAsync(request.AdminId);
            if (admin == null)
            {
                return null;
            }
            var verificationResult = _passwordHasher.VerifyHashedPassword(admin, admin.AdminPassword, request.AdminDtoPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return new AdminDto
            {
                AdminId = admin.AdminId,
                AdminDtoPassword = null
            };
        }
    }
}
