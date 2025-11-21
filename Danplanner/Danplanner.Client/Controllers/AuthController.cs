using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Danplanner.Application.Interfaces.AuthInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.LoginDto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Danplanner.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        // --------------------------
        // Admin registration
        // --------------------------
        [HttpPost("register")]
        public async Task<ActionResult<AdminDto>> Register([FromBody] AdminDto request)
        {
            var admin = await _authService.RegisterAsync(request);
            if (admin == null)
                return BadRequest("Admin already exists.");
            return Ok(admin);
        }

        // --------------------------
        // Admin or User login
        // --------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var token = await _authService.LoginAsync(request);

            if (token == null)
                return BadRequest("Invalid credentials or code.");

            if (token == "OTP_SENT")
                return Ok("OTP sent to your email.");

            // Sign in with cookies for Razor UI
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var userPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", userPrincipal);

            return Ok(token); // Return JWT for API usage
        }

        // --------------------------
        // User login OTP
        // --------------------------
        [HttpPost("user/request-code")]
        public async Task<IActionResult> RequestUserCode([FromBody] RequestCodeDto request)
        {
            var success = await _authService.RequestUserLoginCodeAsync(request.UserEmail);
            if (!success)
                return NotFound("User not found.");
            return Ok("Login code sent to your email.");
        }

        [HttpPost("user/verify-code")]
        public async Task<ActionResult<string>> VerifyUserCode([FromBody] VerifyCodeDto request)
        {
            var token = await _authService.VerifyUserLoginCodeAsync(request.UserEmail, request.Code);
            if (token == null)
                return BadRequest("Invalid code.");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var userPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", userPrincipal);

            return Ok(token);
        }

        // --------------------------
        // User registration OTP
        // --------------------------
        [HttpPost("user/request-register-code")]
        public async Task<IActionResult> RequestUserRegisterCode([FromBody] RequestCodeDto request)
        {
            var success = await _authService.RequestUserRegisterCodeAsync(request.UserEmail);
            if (!success)
                return BadRequest("Kunne ikke sende OTP.");
            return Ok("OTP sendt til din email.");
        }

        [HttpPost("user/verify-register-code")]
        public async Task<IActionResult> VerifyUserRegisterCode([FromBody] VerifyCodeDto request)
        {
            var isValid = _authService.VerifyUserRegisterCode(request.UserEmail, request.Code);
            if (!isValid)
                return BadRequest("Forkert eller udløbet OTP.");

            return Ok("OTP verificeret!");
        }

        [HttpPost("user/register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDto request)
        {
            var user = await _authService.RegisterUserAsync(request);
            if (user == null)
                return BadRequest("Email er allerede i brug.");
            return Ok(user);
        }

        // --------------------------
        // Logout
        // --------------------------
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }

        // --------------------------
        // Authenticated only
        // --------------------------
        [Authorize]
        [HttpGet("authenticated-only")]
        public IActionResult AuthenticatedOnly()
        {
            return Ok($"You are authenticated as {User.Identity?.Name ?? "Unknown"}!");
        }
    }
}
