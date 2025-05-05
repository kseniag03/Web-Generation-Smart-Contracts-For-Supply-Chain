using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Redirect("/login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var token = await _authService.LoginUser(login.Login, login.Password);

                if (token is null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                await SetupCookie(token, login.RememberMe);

                return Ok(token);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("users_login_key") == true)
            {
                return Conflict(new { message = "Login already exists" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            try
            {
                var token = await _authService.RegisterUser(register.Login, register.Password, register.Email);

                if (token is null)
                {
                    return BadRequest(new { message = "Registration failed" });
                }

                await SetupCookie(token);

                return Ok(token);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("users_login_key") == true)
            {
                return Conflict(new { message = "Login already exists" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        // доступ: auditor  OR  deployer  OR  admin
        [Authorize(Roles = "auditor,deployer,admin")]
        [HttpPost("link-metamask")]
        public async Task<IActionResult> LinkMetaMask([FromBody] MetaMaskDto metamask)
        {
            var login = User?.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return BadRequest(new { message = "Null of empty login" });
            }

            var success = await _authService.LinkMetaMask(login, metamask.WalletAddress);

            var userDto = await _authService.GetUserByLogin(login);

            if (userDto is null)
            {
                return BadRequest(new { message = "Cannot extract current user" });
            }

            await SetupCookie(userDto, remember: true);

            return success
                ? Ok(new { message = "MetaMask linked" })
                : BadRequest(new { message = "MetaMask linking failed" });
        }

        // roles allowed to link or relink GitHub
        [Authorize(Roles = "tester,admin")]
        [HttpGet("link-github")]
        public async Task<IActionResult> LinkGitHub()
        {
            Console.WriteLine($"Current user at /link-github: {User?.Identity?.Name}");

            if (User?.Identity is null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var login = User?.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return Unauthorized();
            }

            // уже есть GitHubId? 409 Conflict
            var githubId = await _authService.GetGitHubId(login);

            if (!string.IsNullOrEmpty(githubId))
            {
                return Conflict(new { message = "GitHub already linked" });
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = "/login"
            };

            props.Items["login"] = login;
            props.Items["mode"] = "link"; // на будущее

            return Challenge(props, "GitHub");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            var login = User?.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return BadRequest(new { message = "Null of empty login" });
            }

            var success = await _authService.ChangePassword(login, changePassword.OldPassword, changePassword.NewPassword);

            return success ? Ok(new { message = "Password changed" }) : BadRequest(new { message = "Password change failed" });
        }

        private async Task SetupCookie(UserDto user, bool remember = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role.ToLowerInvariant())
            };

            if (!string.IsNullOrEmpty(user.GitHubId))
            {
                claims.Add(new Claim("urn:github:login", user.GitHubId));
            }

            if (!string.IsNullOrEmpty(user.WalletAddress))
            {
                claims.Add(new Claim("urn:wallet:address", user.WalletAddress));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = remember,          // «запомнить меня»
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true
                }
            );
        }
    }
}
