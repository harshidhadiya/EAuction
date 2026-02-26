using Azure.Core;
using MACUTION.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Data
{
    public class MacutionDatabase :DbContext
    {
        public DbSet<Verifier> verifiers{get;set;}
        public DbSet<Request_admin> request_Admins{get;set;}
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
        public MacutionDatabase(DbContextOptions<MacutionDatabase> options):base(options)
        {        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User{Address="Monvel",createdAt=new DateTime(2026, 2, 26, 14, 44, 45, 31, DateTimeKind.Local).AddTicks(8120),Email="harshid.hadiya@gmail.com",Id=1,MobileNumber=2341,Name="hadiya harshid",role="ADMIN",right_to_add=true,Password="12345"});
            modelBuilder.ApplyConfiguration(new Userconfig());
            modelBuilder.ApplyConfiguration(new Requestconfig());
            modelBuilder.ApplyConfiguration(new Productconfig());
            modelBuilder.ApplyConfiguration(new Imageconfig());
            modelBuilder.ApplyConfiguration(new Verifyconfig());
        }
    }
}