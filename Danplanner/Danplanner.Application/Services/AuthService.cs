using Danplanner.Application.Interfaces.AdminInterfaces;
using Danplanner.Application.Interfaces.AuthInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.LoginDto;
using Danplanner.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace Danplanner.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<Admin> _passwordHasher;
        private readonly IUserGetByEmail _userRepository;
        private readonly IEmailService _emailService;

        // In-memory OTP store
        private static readonly ConcurrentDictionary<string, UserOtp> _userOtps = new();

        public AuthService(
            IAdminRepository adminRepository,
            IUserGetByEmail userRepository,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _passwordHasher = new PasswordHasher<Admin>();
        }

        // --------------------------
        // Token creation
        // --------------------------
        private string CreateTokenForAdmin(Admin admin) => _tokenService.CreateTokenForAdmin(admin);
        private string CreateTokenForUser(UserDto user) => _tokenService.CreateTokenForUser(user);

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

            return new AdminDto
            {
                AdminId = admin.AdminId,
                AdminDtoPassword = null
            };
        }

        // --------------------------
        // Login (Admin & User OTP)
        // --------------------------
        public async Task<string?> LoginAsync(LoginDto request)
        {
            // ----- Admin login -----
            if (request.AdminId.HasValue && !string.IsNullOrEmpty(request.Password))
            {
                var admin = await _adminRepository.GetByIdAsync(request.AdminId.Value);
                if (admin == null) return null;

                var result = _passwordHasher.VerifyHashedPassword(admin, admin.AdminPassword, request.Password);
                if (result == PasswordVerificationResult.Success)
                    return CreateTokenForAdmin(admin);

                return null; // wrong password
            }

            // ----- User OTP login -----
            if (!string.IsNullOrEmpty(request.Email))
            {
                var user = await _userRepository.GetUserByEmailAsync(request.Email);
                if (user == null) return null;

                // Step 1: Request OTP
                if (string.IsNullOrEmpty(request.Code))
                {
                    var code = new Random().Next(100000, 999999).ToString();
                    var otp = new UserOtp
                    {
                        Code = code,
                        Expiration = DateTime.UtcNow.AddMinutes(5)
                    };

                    _userOtps[request.Email] = otp;

                    // Send email
                    await _emailService.SendEmailAsync(
                        request.Email,
                        "Your OTP Code",
                        $"Your OTP code is <b>{code}</b>. It expires in 5 minutes."
                    );

                    return "OTP_SENT";
                }

                // Step 2: Verify OTP
                if (_userOtps.TryGetValue(request.Email, out var storedOtp))
                {
                    if (storedOtp.Expiration < DateTime.UtcNow)
                    {
                        _userOtps.TryRemove(request.Email, out _);
                        return null; // OTP expired
                    }

                    if (storedOtp.Code == request.Code)
                    {
                        _userOtps.TryRemove(request.Email, out _);
                        return CreateTokenForUser(user);
                    }
                }

                return null; // wrong OTP
            }

            return null; // no credentials provided
        }

        // --------------------------
        // Request OTP separately
        // --------------------------
        public async Task<bool> RequestUserLoginCodeAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            var code = new Random().Next(100000, 999999).ToString();
            var otp = new UserOtp
            {
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            };

            _userOtps[email] = otp;

            // Send email
            await _emailService.SendEmailAsync(
                email,
                "Your OTP Code",
                $"Your login OTP code is <b>{code}</b>. It expires in 5 minutes."
            );

            return true;
        }

        // --------------------------
        // Verify OTP separately
        // --------------------------
        public async Task<string?> VerifyUserLoginCodeAsync(string email, string code)
        {
            if (!_userOtps.TryGetValue(email, out var storedOtp))
                return null;

            if (storedOtp.Expiration < DateTime.UtcNow)
            {
                _userOtps.TryRemove(email, out _);
                return null; // expired
            }

            if (storedOtp.Code != code)
                return null; // wrong code

            _userOtps.TryRemove(email, out _);

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return null;

            return CreateTokenForUser(user);
        }

        // --------------------------
        // Helper class for OTP
        // --------------------------
        private class UserOtp
        {
            public string Code { get; set; } = null!;
            public DateTime Expiration { get; set; }
        }
    }
}
