using DXCarsCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace DXCarsCrud.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; } = default!;
        public DbSet<Colors> Colors { get; set; } = default!;
    }
}
