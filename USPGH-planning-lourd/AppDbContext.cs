using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    public class AppDbContext : DbContext
    {
        private DbSet<User> users;
        private DbSet<Role> roles;
        private DbSet<UserRole> userRoles;

        public DbSet<User> Users { get => users; set => users = value; }
        public DbSet<Role> Roles { get => roles; set => roles = value; }
        public DbSet<UserRole> UserRoles { get => userRoles; set => userRoles = value; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

            // Parse connection string or use the one from configuration
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.Create(new Version(10, 5, 9), ServerType.MariaDb),
                mysqlOptions =>
                {
                    // Add retry logic for transient errors
                    mysqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    // Set command timeout to 30 seconds
                    mysqlOptions.CommandTimeout(30);
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the UserRole entity with composite key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.Id); // Using the primary key 'id' from assigned_roles table

            // Configure table names explicitly
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<UserRole>().ToTable("assigned_roles");
            modelBuilder.Entity<User>().ToTable("users");

            // Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.EntityId);
        }

        // Helper method to load user roles
        public void LoadUserRoles(User user)
        {
            // Initialize the roles collection if needed
            if (user.Roles == null)
            {
                user.Roles = new System.Collections.Generic.List<Role>();
            }

            // Clear existing roles and load from database
            user.Roles.Clear();
            var roles = UserRoles
                .Where(ur => ur.EntityId == user.Id && ur.EntityType == "App\\Models\\User")
                .Join(Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                .ToList();

            user.Roles.AddRange(roles);

            // Set the boolean flags based on the roles
            user.IsAdmin = user.Roles.Any(r => r.Name == "admin");
            user.IsSalarie = user.Roles.Any(r => r.Name == "salarie");
        }
    }
}