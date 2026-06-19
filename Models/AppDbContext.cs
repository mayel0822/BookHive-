using Microsoft.EntityFrameworkCore;

namespace BookHive.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MISFacilitator> MISFacilitators { get; set; }
    }
}