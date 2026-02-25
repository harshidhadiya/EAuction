using System.Data;
using MACUTION.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Data
{
    public class MacutionDatabase :DbContext
    {
        public DbSet<Product> products;
        public DbSet<Image> images;
        public DbSet<User> users;
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