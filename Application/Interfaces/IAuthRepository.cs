using Application.Common;

namespace Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserResult> GetUserByLogin(string login);
        Task<UserResult> RegisterUser(string login, string password, string? email);
        Task<UserResult> LoginUser(string login, string password);
        Task<string?> GetGitHubId(string login);
        Task<BoolResult> LinkGitHub(string login, string githubId);
        Task<BoolResult> LinkMetaMask(string login, string walletAddress);
        Task<BoolResult> ChangePassword(string login, string oldPassword, string newPassword);
    }
}
