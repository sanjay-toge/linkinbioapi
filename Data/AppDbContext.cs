using LinkBioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkBioAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<LinkItem> LinkItems { get; set; }
    }
}
