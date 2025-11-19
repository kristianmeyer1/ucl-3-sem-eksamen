using Danplanner.Application.Interfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.LoginDto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Danplanner.Client.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        // Admin registration
        [HttpPost("register")]
        public async Task<ActionResult<AdminDto>> Register([FromBody] AdminDto request)
        {
            var admin = await _authService.RegisterAsync(request);
            if (admin == null)
                return BadRequest("Admin already exists.");
            return Ok(admin);
        }

        // Admin or User login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var token = await _authService.LoginAsync(request);

            if (token == null)
                return BadRequest("Invalid credentials or code.");

            if (token == "OTP_SENT")
                return Ok("OTP sent to your email.");

            // Optionally sign in the user/admin for Razor UI (cookies)
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var userPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", userPrincipal);

            return Ok(token); // Return JWT for API usage
        }

        // User requests OTP login code
        [HttpPost("user/request-code")]
        public async Task<IActionResult> RequestUserCode([FromBody] RequestCodeDto request)
        {
            var success = await _authService.RequestUserLoginCodeAsync(request.UserEmail);
            if (!success)
                return NotFound("User not found.");
            return Ok("Login code sent to your email.");
        }

        // User verifies OTP login code
        [HttpPost("user/verify-code")]
        public async Task<ActionResult<string>> VerifyUserCode([FromBody] VerifyCodeDto request)
        {
            var token = await _authService.VerifyUserLoginCodeAsync(request.UserEmail, request.Code);
            if (token == null)
                return BadRequest("Invalid code.");

            // Optionally sign in for Razor UI
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var userPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", userPrincipal);

            return Ok(token); // Return JWT for API usage
        }

        // Logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // signs out the user
            return RedirectToPage("/Index");  // redirect to home after logout
        }

        // Example endpoint requiring authentication
        [Authorize]
        [HttpGet("authenticated-only")]
        public IActionResult AuthenticatedOnly()
        {
            return Ok($"You are authenticated as {User.Identity?.Name ?? "Unknown"}!");
        }
    }
}
