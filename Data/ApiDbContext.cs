using HotelComparer.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelComparer.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}
