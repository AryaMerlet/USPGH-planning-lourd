using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    public class AppDbContext : DbContext
    {
        private DbSet<User> users;
        private DbSet<Role> roles;
        private DbSet<AssignedRole> assignedRoles;

        public DbSet<User> Users { get => users; set => users = value; }
        public DbSet<Role> Roles { get => roles; set => roles = value; }
        public DbSet<AssignedRole> AssignedRoles { get => assignedRoles; set => assignedRoles = value; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=10.192.136.10;database=uspgh-planning;user=arya;password=Not24get",
                ServerVersion.Create(10, 5, 9, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the composite key for AssignedRole
            modelBuilder.Entity<AssignedRole>()
                .HasKey(ar => new { ar.RoleId, ar.ModelType, ar.ModelId });
        }
    }
}