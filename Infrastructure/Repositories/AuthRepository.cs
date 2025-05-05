using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Helpers;
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

        public async Task<User?> GetUserByLogin(string login)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> RegisterUser(string login, string password, string? email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Login == login))
            {
                return null; // Логин уже занят
            }

            var newUser = new User
            {
                Login = login,
                Firstname = string.Empty,
                Lastname = string.Empty,
                Email = email,
                GitHubId = string.Empty
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync(); // init IdUser

            var passwordHash = _passwordHasher.HashPassword(newUser, password);

            newUser.Userauth = new Userauth
            {
                IdUser = newUser.IdUser,
                PasswordHash = passwordHash
            };

            await RoleHelper.AssignRole(_dbContext, newUser, RoleType.Tester);

            await _dbContext.SaveChangesAsync();

            return newUser;
        }

        public async Task<User?> LoginUser(string login, string password)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .Include(u => u.Wallets)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Userauth.PasswordHash, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<string?> GetGitHubId(string login)
        {
            return await _dbContext.Users
                .Where(u => u.Login == login || u.Email == login)
                .Select(u => u.GitHubId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> LinkGitHub(string login, string githubId)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return false;
            }

            user.GitHubId = githubId;

            if (!string.IsNullOrEmpty(user.GitHubId))
            {
                await RoleHelper.AssignRole(_dbContext, user, RoleType.Auditor);
            }

            var entry = _dbContext.Entry(user);

            Console.WriteLine(entry.State);                            // должен быть Modified
            Console.WriteLine(entry.Property(u => u.GitHubId).IsModified); // должен быть true

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LinkMetaMask(string login, string walletAddress)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .Include(u => u.Wallets)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return false;
            }

            var walletExists = await _dbContext.Wallets
                .AnyAsync(w => w.Address == walletAddress);

            if (walletExists)
            {
                Console.WriteLine("This wallet address is already linked");

                return false;
            }

            var wallet = new Wallet
            {
                IdUser = user.IdUser,
                Address = walletAddress,
                CreatedAt = DateTime.UtcNow
            };

            user.Wallets.Add(wallet);

            await RoleHelper.AssignRole(_dbContext, user, RoleType.Deployer);

            var entry = _dbContext.Entry(user);

            Console.WriteLine(entry.State);                            // должен быть Modified
            Console.WriteLine(entry.Property(u => u.GitHubId).IsModified); // должен быть true

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePassword(string login, string oldPassword, string newPassword)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Userauth.PasswordHash, oldPassword);

            if (result != PasswordVerificationResult.Success)
            {
                Console.WriteLine("Wrong password");

                return false; // Старый пароль неверный
            }

            user.Userauth.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
