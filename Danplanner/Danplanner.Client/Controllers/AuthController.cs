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
using Microsoft.AspNetCore.Authorization;

namespace Danplanner.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (IAuthService authService): ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;


        [HttpPost("register")]
        public async Task<ActionResult<Admin>> Register([FromBody] AdminDto request)
        {
            var admin = await authService.RegisterAsync(request);
            if (admin == null)
                return BadRequest("User already exists");
            return Ok(admin);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] AdminDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token == null)
                return BadRequest("Invalid credentials");
            return Ok(token);
        }
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }

        private string CreateToken(AdminDto admin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.AdminId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString())
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
