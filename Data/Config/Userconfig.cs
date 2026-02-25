using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MACUTION.Data.Config
{
    public class Userconfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> option)
        {

            option.HasKey(x => x.Id);
            option.Property(x => x.createdAt).HasDefaultValueSql("GetDate()");
            option.Property(x => x.Name).IsRequired();
            option.Property(x => x.Email).IsRequired();
            option.HasIndex(x => x.Email).IsUnique();
            option.Property(X=>X.MobileNumber).IsRequired();
            option.Property(x=>x.role).HasDefaultValue("USER");
            option.Property(x=>x.ProfileImageUrl).HasDefaultValue("");
            option.Property(x=>x.Password).IsRequired();
            option.Property(x=>x.Address).IsRequired();
            
        }
    }
}