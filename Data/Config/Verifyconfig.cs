using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MACUTION.Data.Config
{
    public class Verifyconfig : IEntityTypeConfiguration<Verifier>
    {
        public void Configure(EntityTypeBuilder<Verifier> builder)
        {
            builder.Property(x=>x.isverified).HasDefaultValue(false);
            builder.Property(x=>x.verifier_id).IsRequired();
            builder.HasOne(x=>x.user).WithMany(v=>v.verifiers).HasForeignKey(x=>x.verifier_id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x=>x.product).WithOne(y=>y.verifier).HasForeignKey<Verifier>(v=>v.product_id).OnDelete(DeleteBehavior.SetNull);
            builder.Property(x=>x.verificationDate).IsRequired();

        }
    }
}