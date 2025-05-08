using Application.Common;
using Application.Interfaces;

namespace Application.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<UserResult> GetUserByLogin(string login)
        {
            return await _authRepository.GetUserByLogin(login);
        }

        public async Task<UserResult> RegisterUser(string login, string password, string? email)
        {
            return await _authRepository.RegisterUser(login, password, email);
        }

        public async Task<UserResult> LoginUser(string login, string password)
        {
            return await _authRepository.LoginUser(login, password);
        }

        public async Task<string?> GetGitHubId(string login)
        {
            return await _authRepository.GetGitHubId(login);
        }

        public async Task<BoolResult> LinkGitHub(string login, string githubId)
        {
            return await _authRepository.LinkGitHub(login, githubId);
        }

        public async Task<BoolResult> LinkMetaMask(string login, string walletAddress)
        {
            return await _authRepository.LinkMetaMask(login, walletAddress);
        }

        public async Task<BoolResult> ChangePassword(string login, string oldPassword, string newPassword)
        {
            return await _authRepository.ChangePassword(login, oldPassword, newPassword);
        }
    }
}
