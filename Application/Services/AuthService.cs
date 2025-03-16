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

        public async Task<bool> RegisterUser(string username, string password, string? email = null)
        {
            return await _authRepository.RegisterUser(username, email, password);
        }

        public async Task<User?> LoginUser(string username, string password)
        {
            return await _authRepository.LoginUser(username, password);
        }

        public async Task<bool> LinkGitHub(string username, string githubId)
        {
            return await _authRepository.LinkGitHub(username, githubId);
        }

        public async Task<bool> LinkMetaMask(string username, string walletAddress)
        {
            return await _authRepository.LinkMetaMask(username, walletAddress);
        }

        public async Task<bool> ChangePassword(string username, string oldPassword, string newPassword)
        {
            return await _authRepository.ChangePassword(username, oldPassword, newPassword);
        }

        private bool IsAuthorized(string userRole, string requiredRole)
        {
            var roleHierarchy = new[] { "User", "Tester", "Auditor", "Deployer", "Admin" };

            return Array.IndexOf(roleHierarchy, userRole) >= Array.IndexOf(roleHierarchy, requiredRole);
        }
    }
}
