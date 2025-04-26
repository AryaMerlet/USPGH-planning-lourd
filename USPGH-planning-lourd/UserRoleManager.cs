using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    public static class UserRoleManagerExtensions
    {
        // This extension method adds the functionality to the AppDbContext class
        public static List<Role> GetUserRoles(this AppDbContext dbContext, User user)
        {
            return dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id && ur.ModelType == "App\\Models\\User")
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToList();
        }

        public static bool IsUserInRole(this AppDbContext dbContext, User user, string roleName)
        {
            if (user.Roles.Count == 0)
            {
                dbContext.LoadUserRoles(user);
            }

            return user.IsInRole(roleName);
        }
    }
}