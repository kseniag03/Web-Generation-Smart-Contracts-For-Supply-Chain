using Application.DTOs;
using Application.Mappings;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<UserDto?> RegisterUser(string login, string password, string? email)
        {
            var user = await _authRepository.RegisterUser(login, password, email);

            return user?.ToDto();
        }

        public async Task<UserDto?> LoginUser(string login, string password)
        {
            var user = await _authRepository.LoginUser(login, password);

            return user?.ToDto();
        }

        public async Task<bool> LinkGitHub(string login, string githubId)
        {
            return await _authRepository.LinkGitHub(login, githubId);
        }

        public async Task<bool> LinkMetaMask(string login, string walletAddress)
        {
            return await _authRepository.LinkMetaMask(login, walletAddress);
        }

        public async Task<bool> ChangePassword(string login, string oldPassword, string newPassword)
        {
            return await _authRepository.ChangePassword(login, oldPassword, newPassword);
        }

        private bool IsAuthorized(string userRole, string requiredRole)
        {
            var roleHierarchy = new[] { "User", "Tester", "Auditor", "Deployer", "Admin" };

            return Array.IndexOf(roleHierarchy, userRole) >= Array.IndexOf(roleHierarchy, requiredRole);
        }
    }
}
