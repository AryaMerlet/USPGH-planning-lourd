using Microsoft.EntityFrameworkCore;
using USPGH_planning_lourd.classes;

namespace USPGH_planning_lourd
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get => users; set => users = value; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            object value = optionsBuilder.UseMySql("server=10.192.136.10;database=uspgh-planning;user=arya;password=Not24get",
                new MySqlServerVersion(new Version(10, 5, 9)));
        }
    }
}
