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
        public DbSet<User> Users { get => users; set => users = value; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

            // Parse connection string or use the one from configuration
            optionsBuilder.UseMySql(connectionString,
                ServerVersion.Create(new Version(10, 5, 9), ServerType.MariaDb));
        }
    }
}