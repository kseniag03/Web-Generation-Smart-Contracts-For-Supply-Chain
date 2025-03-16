using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        /*
         * 
        

        [HttpGet("get-role")]
        [Authorize]
        public IActionResult GetUserRole()
        {
            var role = _authService.GetUserRole(User.Identity.Name);
            return Ok(role);
        }
        
        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User { UserName = model.Username, WalletAddress = model.WalletAddress };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User created successfully!" });
        }

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
        }

        public async Task AssignRoleByOAuthAsync(Guid userId, string? walletAddress, string? githubId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            bool hasGitHub = !string.IsNullOrEmpty(githubId);
            bool hasMetaMask = !string.IsNullOrEmpty(walletAddress);

            user.GitHubId = hasGitHub ? githubId : user.GitHubId;
            user.WalletAddress = hasMetaMask ? walletAddress : user.WalletAddress;

            // Определение роли
            if (hasGitHub && hasMetaMask)
                user.Role = RoleType.Deployer;
            else if (hasGitHub)
                user.Role = RoleType.Auditor;

            await _dbContext.SaveChangesAsync();
        }

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

        [HttpPost("auth/github")]
        public async Task<IActionResult> LoginWithGitHub([FromBody] GitHubLoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.GitHubId == request.GitHubId);
            if (user == null)
            {
                user = new User { GitHubId = request.GitHubId, Role = RoleType.Auditor };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            return Ok(new { Message = "Logged in as " + user.Role });
        }

        [HttpPost("auth/metamask")]
        public async Task<IActionResult> LoginWithMetaMask([FromBody] MetaMaskLoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.WalletAddress == request.Wallet);

            if (user == null)
            {
                user = new User { WalletAddress = request.Wallet, Role = RoleType.Deployer };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            return Ok(new { Message = "Logged in as " + user.Role });
        }*/
    }
}
