using Application.DTOs;
using Application.Services;
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

                return Ok(token);
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
                    BadRequest("Registration failed");
                }

                return Ok(token);
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

        [HttpPost("link-github")]
        [Authorize]
        public async Task<IActionResult> LinkGithub([FromBody] GithubDto github)
        {
            var success = await _authService.LinkGitHub(User.Identity.Name, github.GithubLogin);
            return success ? Ok("GitHub linked") : BadRequest("GitHub linking failed");
        }

        [HttpPost("link-metamask")]
        [Authorize]
        public async Task<IActionResult> LinkMetaMask([FromBody] MetaMaskDto metamask)
        {
            var success = await _authService.LinkMetaMask(User.Identity.Name, metamask.WalletAddress);
            return success ? Ok("MetaMask linked") : BadRequest("MetaMask linking failed");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            var success = await _authService.ChangePassword(User.Identity.Name, changePassword.OldPassword, changePassword.NewPassword);
            return success ? Ok("Password changed") : BadRequest("Password change failed");
        }
    }
}
