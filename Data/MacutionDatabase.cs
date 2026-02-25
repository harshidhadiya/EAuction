using MACUTION.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Data
{
    public class MacutionDatabase :DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
        public MacutionDatabase(DbContextOptions<MacutionDatabase> options):base(options)
        {        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Userconfig());
            modelBuilder.ApplyConfiguration(new Productconfig());
            modelBuilder.ApplyConfiguration(new Imageconfig());
            modelBuilder.ApplyConfiguration(new Verifyconfig());
        }
    }
}