using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MACUTION.Data.Config
{
    public class Imageconfig : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.Property(x=>x.ProductId).IsRequired();
            builder.HasOne(x=>x.product).WithMany(i=>i.Images).HasForeignKey(y=>y.ProductId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}