using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthRepository: IAuthRepository
    {
        private readonly ContractsDbContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthRepository(ContractsDbContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User?> GetByUsernameAsync(string login)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User?> RegisterUser(string login, string password, string? email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Login == login))
                return null; // Логин уже занят

            var id = _dbContext.Users.Count() + 1;

            var newUser = new User
            {
                IdUser = id, // Guid.NewGuid(),
                Login = login,
                Firstname = string.Empty,
                Lastname = string.Empty,
                Email = email
                // GitHubId = null              need to add
            };

            var passwordHash = _passwordHasher.HashPassword(newUser, password);

            newUser.Userauth = new Userauth
            {
                IdUser = newUser.IdUser,
                PasswordHash = passwordHash
            };

            var testerRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.IdRole == (int)RoleType.Tester);

            if (testerRole != null)
            {
                newUser.IdRoles.Add(testerRole);
            }

            _dbContext.Users.Add(newUser);

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser;
        }

        public async Task<User?> LoginUser(string login, string password)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Userauth.PasswordHash, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<bool> LinkGitHub(string login, string githubId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null) return false;

            /*
            user.GitHubId = githubId;

            // Если GitHub привязан, повышаем роль до Auditor
            if (!string.IsNullOrEmpty(user.GitHubId) && string.IsNullOrEmpty(user.WalletAddress))
                user.Role = RoleType.Auditor;

            await _dbContext.SaveChangesAsync();*/
            return true;
        }

        public async Task<bool> LinkMetaMask(string login, string walletAddress)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null) return false;

            /*
            user.WalletAddress = walletAddress;

            // Если GitHub и MetaMask привязаны, даем роль Deployer
            if (!string.IsNullOrEmpty(user.GitHubId) && !string.IsNullOrEmpty(user.WalletAddress))
                user.Role = RoleType.Deployer;

            await _dbContext.SaveChangesAsync();*/
            return true;
        }

        public async Task<bool> ChangePassword(string login, string oldPassword, string newPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user == null) return false;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Userauth.PasswordHash, oldPassword);

            if (result != PasswordVerificationResult.Success)
                return false; // Старый пароль неверный

            user.Userauth.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
