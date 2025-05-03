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

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var login = User.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByLogin(login);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Ok();
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
                    return Unauthorized("Invalid credentials");
                }

                await SetupCookie(token);

                return Ok(token);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("users_login_key") == true)
            {
                return Conflict("Login already exists");
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
                    return BadRequest("Registration failed");
                }

                await SetupCookie(token);

                return Ok(token);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("users_login_key") == true)
            {
                return Conflict("Login already exists");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        [HttpGet("check-username")]
        public IActionResult CheckUsername(string name)
        {
            bool exists = false; // _authService.IsUsernameTaken(name);
            return exists ? Conflict("Username already taken") : Ok();
        }
        /*
        [HttpPost("link-github")]
        [Authorize]
        public async Task<IActionResult> LinkGithub([FromBody] GithubDto github)
        {
            var success = await _authService.LinkGitHub(User.Identity.Name, github.GithubLogin);
            return success ? Ok("GitHub linked") : BadRequest("GitHub linking failed");
        }*/

        [Authorize]
        [HttpPost("link-metamask")]
        public async Task<IActionResult> LinkMetaMask([FromBody] MetaMaskDto metamask)
        {
            var login = User?.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return BadRequest("Null of empty login");
            }

            var success = await _authService.LinkMetaMask(login, metamask.WalletAddress);

            return success ? Ok("MetaMask linked") : BadRequest("MetaMask linking failed");
        }

        // [Authorize] // обязательно!
        [HttpGet("link-github")]
        public IActionResult LinkGitHub()
        {
            Console.WriteLine($"Current user at /link-github: {User?.Identity?.Name}");

            if (User?.Identity is null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = "/oauth-github"
            };

            // сохраняем логин текущего пользователя явно
            var login = User?.Identity?.Name;

            if (!string.IsNullOrEmpty(login))
            {
                props.Items["login"] = login;
                props.Items["mode"] = "link"; // на будущее
            }
            else
            {
                return Unauthorized();
            }

            return Challenge(props, "GitHub");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            var login = User?.Identity?.Name;

            if (string.IsNullOrEmpty(login))
            {
                return BadRequest("Null of empty login");
            }

            var success = await _authService.ChangePassword(login, changePassword.OldPassword, changePassword.NewPassword);

            return success ? Ok("Password changed") : BadRequest("Password change failed");
        }

        private async Task SetupCookie(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
