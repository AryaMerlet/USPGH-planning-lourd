using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    public class UserRoleManager
    {
        private readonly AppDbContext _dbContext;

        public UserRoleManager(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Role> GetUserRoles(User user)
        {
            return _dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id && ur.ModelType == "App\\Models\\User")
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToList();
        }

        public void LoadUserRoles(User user)
        {
            user.Roles = GetUserRoles(user);
        }

        public bool IsUserInRole(User user, string roleName)
        {
            if (user.Roles.Count == 0)
            {
                LoadUserRoles(user);
            }

            return user.IsInRole(roleName);
        }
    }
}