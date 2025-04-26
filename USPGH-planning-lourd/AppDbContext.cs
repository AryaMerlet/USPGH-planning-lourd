using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;
using System.Configuration;

namespace USPGH_planning_lourd
{
    public class AppDbContext : DbContext
    {
        private DbSet<User> users;
        public DbSet<User> Users { get => users; set => users = value; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<AssignedRole> AssignedRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            optionsBuilder.UseMySql("server=10.192.136.10;database=uspgh-planning;user=arya;password=Not24get",
                new MySqlServerVersion(new Version(10, 5, 9)));


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the entity type for User
            modelBuilder.Entity<User>()
                .ToTable("users");

            // Configure the entity type for Role
            modelBuilder.Entity<Role>()
                .ToTable("roles");

            // Configure the entity type for AssignedRole
            modelBuilder.Entity<AssignedRole>()
                .ToTable("assigned_roles");

            // Define the relationship between User and Role through AssignedRole
            modelBuilder.Entity<AssignedRole>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(ar => ar.RoleId);
        }
    }
}