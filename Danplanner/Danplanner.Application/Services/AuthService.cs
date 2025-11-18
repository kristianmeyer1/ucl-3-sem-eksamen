using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Danplanner.Application.Interfaces;
using Danplanner.Application.Interfaces.AdminInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.LoginDto;
using Danplanner.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Danplanner.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<Admin> _passwordHasher;

        // Temporary in-memory OTP store
        private static readonly ConcurrentDictionary<string, string> _userOtps = new();

        public AuthService(IAdminRepository adminRepository, IUserRepository userRepository, ITokenService tokenService)
        {
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<Admin>();
        }

        // --------------------------
        // Token creation
        // --------------------------
        private string CreateTokenForAdmin(Admin admin)
        {
            return _tokenService.CreateTokenForAdmin(admin);
        }

        private string CreateTokenForUser(UserDto user)
        {
            return _tokenService.CreateTokenForUser(user);
        }

        // --------------------------
        // Admin Registration
        // --------------------------
        public async Task<AdminDto> RegisterAsync(AdminDto request)
        {
            var existingAdmin = await _adminRepository.GetByIdAsync(request.AdminId);
            if (existingAdmin != null)
                return null;

            var admin = new Admin
            {
                AdminId = request.AdminId,
                AdminPassword = _passwordHasher.HashPassword(null, request.AdminDtoPassword)
            };

            await _adminRepository.AddAsync(admin);

            // Return DTO (without password)
            return new AdminDto
            {
                AdminId = admin.AdminId,
                AdminDtoPassword = null
            };
        }

        // --------------------------
        // Login (Admin & User)
        // --------------------------
        public async Task<string?> LoginAsync(LoginDto request)
        {
            // ----- Admin Login -----
            if (!string.IsNullOrEmpty(request.AdminId) && !string.IsNullOrEmpty(request.Password))
            {
                var admin = await _adminRepository.GetByIdAsync(Convert.ToInt32(request.AdminId));
                if (admin == null) return null;

                var result = _passwordHasher.VerifyHashedPassword(null, admin.AdminPassword, request.Password);
                if (result == PasswordVerificationResult.Success)
                    return CreateTokenForAdmin(admin); // Use domain entity

                return null;
            }

            // ----- User Login via OTP -----
            if (!string.IsNullOrEmpty(request.Email))
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null) return null;

                // Step 1: Request OTP
                if (string.IsNullOrEmpty(request.Code))
                {
                    var code = new Random().Next(100000, 999999).ToString();
                    _userOtps[request.Email] = code;

                    // For testing, print OTP to console
                    Console.WriteLine($"OTP for {request.Email}: {code}");
                    return "OTP_SENT";
                }

                // Step 2: Verify OTP
                if (_userOtps.TryGetValue(request.Email, out var storedCode) && storedCode == request.Code)
                {
                    _userOtps.TryRemove(request.Email, out _);
                    return CreateTokenForUser(user); // Use domain entity
                }

                return null; // Wrong OTP
            }

            return null;
        }

        // --------------------------
        // Optional: Request OTP separately
        // --------------------------
        public async Task<bool> RequestUserLoginCodeAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            var code = new Random().Next(100000, 999999).ToString();
            _userOtps[email] = code;

            Console.WriteLine($"OTP for {email}: {code}");
            return true;
        }

        // --------------------------
        // Optional: Verify OTP separately
        // --------------------------
        public async Task<string?> VerifyUserLoginCodeAsync(string email, string code)
        {
            if (!_userOtps.TryGetValue(email, out var storedCode) || storedCode != code)
                return null;

            _userOtps.TryRemove(email, out _);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return CreateTokenForUser(user);
        }
    }
}
