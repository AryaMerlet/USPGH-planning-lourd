using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    public class AppDbContext : DbContext
    {
        private DbSet<User>? users;
        private DbSet<Role>? roles;
        private DbSet<UserRole>? userRoles;

        public DbSet<User> Users { get => users!; set => users = value; }
        public DbSet<Role> Roles { get => roles!; set => roles = value; }
        public DbSet<UserRole> UserRoles { get => userRoles!; set => userRoles = value; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=10.192.136.10;database=uspgh-planning;user=arya;password=Not24get",
                ServerVersion.Create(10, 5, 9, ServerType.MariaDb));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the composite key for UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.RoleId, ur.ModelType, ur.UserId });

            // Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);
        }
    }
}