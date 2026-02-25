using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MACUTION.Data.Config
{
    public class Productconfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> option)
        {
            
         option.HasKey(x=>x.Id);
         option.Property(x=>x.product_name).IsRequired();
         option.Property(x=>x.Buy_Date).IsRequired();
         option.HasOne(x=>x.user).WithMany(u=>u.products).HasForeignKey(k=>k.user_id).OnDelete(DeleteBehavior.SetNull);
     

        }
    }
}