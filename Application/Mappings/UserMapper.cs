using Application.DTOs;
using Core.Entities;
using Core.Enums;

namespace Application.Mappings
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.IdUser,
                Login = user.Login,
                Email = user.Email,
                Role = user.IdRoles.Select(r => ((RoleType)r.IdRole).ToString()).FirstOrDefault()
            };
        }
    }
}
