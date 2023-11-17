using FronyToBack.Models;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Product> Products { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    
    }
}
