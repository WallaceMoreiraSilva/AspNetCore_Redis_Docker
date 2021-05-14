using AspNetCore_Redis_Docker.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore_Redis_Docker.Data
{
    public class ContextBase : DbContext
    {
        public ContextBase(DbContextOptions<ContextBase> options)
           : base(options)
        {
        }
        public DbSet<Pais> Paises { get; set; }
    }
}
