using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TP1_ARQWEB.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using TP1_ARQWEB.Models;


namespace TP1_ARQWEB.Data
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<InfectionReport> InfectionReport { get; set; }
        public DbSet<NegativeTest> NegativeTest { get; set; }


        public DbSet<Location> Location { get; set; }
        public DbSet<Stay> Stay { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}