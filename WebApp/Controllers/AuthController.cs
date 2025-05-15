using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                return StatusCode(500, $"Logout error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var result = await _authService.LoginUser(login.Login, login.Password);

                if (!result.Succeeded || result.Payload is null)
                {
                    return Unauthorized(new { message = result.Error ?? "Invalid credentials" });
                }

                var token = result.Payload;

                await SetupCookie(token, login.RememberMe);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Login error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            try
            {
                var result = await _authService.RegisterUser(register.Login, register.Password, register.Email);

                if (!result.Succeeded || result.Payload is null)
                {
                    return BadRequest(new { message = result.Error ?? "Registration failed" });
                }

                var token = result.Payload;

                await SetupCookie(token);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Register error: {ex.Message}");
            }
        }

        [Authorize(Roles = "auditor,deployer,admin")]
        [HttpPost("link-metamask")]
        public async Task<IActionResult> LinkMetaMask([FromBody] MetaMaskDto metamask)
        {
            try
            {
                var login = User?.Identity?.Name;

                if (string.IsNullOrEmpty(login))
                {
                    return Unauthorized(new { message = "Null of empty login" });
                }

                var result = await _authService.LinkMetaMask(login, metamask.WalletAddress);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = result.Error ?? "MetaMask linking failed" });
                }

                var userResult = await _authService.GetUserByLogin(login);

                if (!userResult.Succeeded || userResult.Payload is null)
                {
                    return Unauthorized(new { message = result.Error ?? "Cannot extract current user" });
                }

                var token = userResult.Payload;

                await SetupCookie(token, remember: true);

                return Ok(new { message = "MetaMask linked" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Link MetaMask error: {ex.Message}");
            }
        }

        [Authorize(Roles = "tester,admin")]
        [HttpGet("link-github")]
        public async Task<IActionResult> LinkGitHub()
        {
            try
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

                return Challenge(props, "GitHub");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Link GitHub error: {ex.Message}");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            try
            {
                var login = User?.Identity?.Name;

                if (string.IsNullOrEmpty(login))
                {
                    return BadRequest(new { message = "Null or empty login" });
                }

                var result = await _authService.ChangePassword(login, changePassword.OldPassword, changePassword.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = result.Error ?? "Password change failed" });
                }

                var userResult = await _authService.GetUserByLogin(login);

                if (!userResult.Succeeded || userResult.Payload is null)
                {
                    return Unauthorized(new { message = result.Error ?? "Cannot extract current user" });
                }

                var token = userResult.Payload;

                await SetupCookie(token, remember: true);

                return Ok(new { message = "Password changed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Password change error: {ex.Message}");
            }
        }

        private async Task SetupCookie(UserDto user, bool remember = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role.ToLowerInvariant())
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim("urn:email:login", user.Email));
            }

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
                    IsPersistent = remember,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true
                }
            );
        }
    }
}
