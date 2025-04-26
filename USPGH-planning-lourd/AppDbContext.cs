using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;
using System;
using System.Linq;

namespace USPGH_planning_lourd
{
    public class AppDbContext : DbContext
    {
        private DbSet<User> users;
        public DbSet<User> Users { get => users; set => users = value; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=10.192.136.10;database=uspgh-planning;user=arya;password=Not24get",
                ServerVersion.Parse("10.5.9-mariadb"));
        }

        // Load user roles by querying the model_has_roles junction table
        public void LoadUserRoles(User user)
        {
            // Query to check if user has admin role
            var adminQuery = Database.ExecuteSqlRaw(
                "SELECT COUNT(*) FROM model_has_roles " +
                "JOIN roles ON model_has_roles.role_id = roles.id " +
                "WHERE model_has_roles.model_id = {0} " +
                "AND model_has_roles.model_type = 'App\\\\Models\\\\User' " +
                "AND roles.name = 'admin'",
                user.Id);

            // Query to check if user has salarie role
            var salarieQuery = Database.ExecuteSqlRaw(
                "SELECT COUNT(*) FROM model_has_roles " +
                "JOIN roles ON model_has_roles.role_id = roles.id " +
                "WHERE model_has_roles.model_id = {0} " +
                "AND model_has_roles.model_type = 'App\\\\Models\\\\User' " +
                "AND roles.name = 'salarie'",
                user.Id);

            // Set the role flags based on the query results
            user.IsAdmin = adminQuery > 0;
            user.IsSalarie = salarieQuery > 0;
        }
    }
}