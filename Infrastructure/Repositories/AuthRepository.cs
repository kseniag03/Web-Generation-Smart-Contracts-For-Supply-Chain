using Application.Common;
using Application.Mappings;
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

        public async Task<UserResult> GetUserByLogin(string login)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return UserResult.Fail("User not found or authorization data is missing");
            }

            return UserResult.Ok(user.ToDto());
        }

        public async Task<UserResult> RegisterUser(string login, string password, string? email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Login == login))
            {
                return UserResult.Fail("Login is already taken");
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
            await _dbContext.SaveChangesAsync();

            var passwordHash = _passwordHasher.HashPassword(newUser, password);

            newUser.Userauth = new Userauth
            {
                IdUser = newUser.IdUser,
                PasswordHash = passwordHash
            };

            await RoleHelper.AssignRole(_dbContext, newUser, RoleType.Tester);
            await _dbContext.SaveChangesAsync();

            return UserResult.Ok(newUser.ToDto());
        }

        public async Task<UserResult> LoginUser(string login, string password)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .Include(u => u.Wallets)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return UserResult.Fail("User not found or authorization data is missing");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Userauth.PasswordHash, password);

            return result == PasswordVerificationResult.Success
                ? UserResult.Ok(user.ToDto())
                : UserResult.Fail("Wrong password");
        }

        public async Task<string?> GetGitHubId(string login)
        {
            return await _dbContext.Users
                .Where(u => u.Login == login || u.Email == login)
                .Select(u => u.GitHubId)
                .FirstOrDefaultAsync();
        }

        public async Task<BoolResult> LinkGitHub(string login, string githubId)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return BoolResult.Fail("User not found or authorization data is missing");
            }

            if (string.IsNullOrEmpty(githubId))
            {
                return BoolResult.Fail("Provided Github Id is empty");
            }

            user.GitHubId = githubId;

            await RoleHelper.AssignRole(_dbContext, user, RoleType.Auditor);
            await _dbContext.SaveChangesAsync();

            return BoolResult.Ok();
        }

        public async Task<BoolResult> LinkMetaMask(string login, string walletAddress)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .Include(u => u.IdRoles)
                .Include(u => u.Wallets)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return BoolResult.Fail("User not found or authorization data is missing");
            }

            if (string.IsNullOrEmpty(walletAddress))
            {
                return BoolResult.Fail("Provided Wallet Address is empty");
            }

            var walletExists = await _dbContext.Wallets
                .AnyAsync(w => w.Address == walletAddress);

            if (walletExists)
            {
                return BoolResult.Fail("Provided Wallet Address is already linked");
            }

            var wallet = new Wallet
            {
                IdUser = user.IdUser,
                Address = walletAddress,
                CreatedAt = DateTime.UtcNow
            };

            user.Wallets.Add(wallet);

            await RoleHelper.AssignRole(_dbContext, user, RoleType.Deployer);
            await _dbContext.SaveChangesAsync();

            return BoolResult.Ok();
        }

        public async Task<BoolResult> ChangePassword(string login, string oldPassword, string newPassword)
        {
            var user = await _dbContext.Users
                .Include(u => u.Userauth)
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

            if (user == null || user.Userauth == null)
            {
                return BoolResult.Fail("User not found or authorization data is missing");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Userauth.PasswordHash, oldPassword);

            if (result != PasswordVerificationResult.Success)
            {
                return BoolResult.Fail("Wrong password");
            }

            user.Userauth.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            
            await _dbContext.SaveChangesAsync();

            return BoolResult.Ok();
        }
    }
}
