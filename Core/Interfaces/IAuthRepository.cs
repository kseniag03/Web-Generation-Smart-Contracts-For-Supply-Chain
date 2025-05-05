using Core.Entities;

namespace Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByLogin(string login);
        Task<User?> RegisterUser(string login, string password, string? email);
        Task<User?> LoginUser(string login, string password);
        Task<string?> GetGitHubId(string login);
        Task<bool> LinkGitHub(string login, string githubId);
        Task<bool> LinkMetaMask(string login, string walletAddress);
        Task<bool> ChangePassword(string login, string oldPassword, string newPassword);
    }
}
