using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Redis_Docker.Models
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
           : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
    }
}
