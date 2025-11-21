using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Danplanner.Application.Interfaces.AuthInterfaces;
using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Models.LoginDto;
using Danplanner.Application.Services;
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
        private readonly IUserGetByEmail _userGetByEmail;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, IUserGetByEmail userGetByEmail, ITokenService tokenService)
        {
            _authService = authService;
            _userGetByEmail = userGetByEmail;
            _tokenService = tokenService;
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
                return Ok(new { status = "OTP_SENT" });

            // Sign in with cookie
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "Cookies"); // Use cookie scheme
            var userPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", userPrincipal);

            return Ok(new { status = "OK", token });
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

            // Auto-login after OTP verification
            var user = await _userGetByEmail.GetUserByEmailAsync(request.UserEmail);
            if (user == null)
                return NotFound("User not found.");

            var token = _tokenService.CreateTokenForUser(user); // Generate JWT token
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "Cookies"); // cookie scheme
            var userPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", userPrincipal);

            return Ok(new { status = "OK", token });
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
