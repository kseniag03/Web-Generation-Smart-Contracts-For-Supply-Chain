using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Helpers
{
    internal static class RoleHelper
    {
        public static async Task AssignRole(ContractsDbContext db, User user, RoleType type)
        {
            var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == type.ToString());

            if (role != null && !user.IdRoles.Contains(role))
            {
                user.IdRoles.Add(role);
            }
        }
    }
}
