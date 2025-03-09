using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {/*
            var user = new User { UserName = model.Username, WalletAddress = model.WalletAddress };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);*/

            return Ok(new { message = "User created successfully!" });
        }
        /*
        /// <summary>
        /// Назначить роль пользователю
        /// </summary>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null) return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Role assigned successfully.");
        }*/

        /// <summary>
        /// Получить текущего пользователя
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            return Ok(new { user.Username, user.WalletAddress });
        }
    }
}
