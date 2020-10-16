using Microsoft.EntityFrameworkCore;
using TP1_ARQWEB.Models;

namespace TP1_ARQWEB.Data
{
    public class MvcLocationContext : DbContext
    {
        public MvcLocationContext(DbContextOptions<MvcLocationContext> options)
            : base(options)
        {
        }

        public DbSet<Location> Location { get; set; }
    }
}