using Application.DTOs;
using Core.Entities;
using Core.Enums;

namespace Application.Mappings
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            var maxRole = user.IdRoles
                .Select(r => Enum.TryParse<RoleType>(r.RoleName, out var rt) ? rt : RoleType.User)
                .DefaultIfEmpty(RoleType.User)
                .Max();

            return new UserDto
            {
                Id = user.IdUser,
                Login = user.Login,
                Email = user.Email,
                Role = maxRole.ToString(),
                GitHubId = user.GitHubId,
                WalletAddress = user.Wallets
                    .Where(w => w.IdUser == user.IdUser && !string.IsNullOrEmpty(w.Address))
                    .Select(w => w.Address)
                    .FirstOrDefault(),
            };
        }
    }
}
