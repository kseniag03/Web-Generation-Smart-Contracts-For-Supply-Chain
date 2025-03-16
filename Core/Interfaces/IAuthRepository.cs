using Core.Entities;

namespace Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> RegisterUser(string username, string email, string password);
        Task<User?> LoginUser(string username, string password);
        Task<bool> LinkGitHub(string username, string githubId);
        Task<bool> LinkMetaMask(string username, string walletAddress);
        Task<bool> ChangePassword(string username, string oldPassword, string newPassword);
    }
}
