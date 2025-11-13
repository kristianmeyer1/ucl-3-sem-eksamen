using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Danplanner.Domain.Entities;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Identity;
using Danplanner.Application.Interfaces;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;

namespace Danplanner.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdminDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AdminDtoPassword))
                return BadRequest("Invalid request");

            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AdminDtoPassword))
                return BadRequest("Invalid request");

            var admin = await _authService.LoginAsync(request);

            if (admin == null)
                return Unauthorized("Invalid credentials");

            // Here you would generate a JWT token; for now, a placeholder string:
            string token = CreateToken(admin);

            return Ok(token);
        }

        private string CreateToken(AdminDto admin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.AdminId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }

}
