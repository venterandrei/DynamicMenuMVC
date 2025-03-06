using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebApplicationMenu.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts)
        {
        }

        public DbSet<SiteApplication> SiteApplications { get; set; }
    }
}
