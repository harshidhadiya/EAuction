using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MACUTION.Data.Config
{
    public class Requestconfig : IEntityTypeConfiguration<Request_admin>
    {
        public void Configure(EntityTypeBuilder<Request_admin> option)
        {
            option.HasKey(x=>x.Id);
            option.Property(x=>x.request_person_id).IsRequired();
            option.Property(x=>x.verified_admin).HasDefaultValue(false);
            option.HasOne(x=>x.requet_person).WithOne(x=>x.request).HasForeignKey<Request_admin>(y=>y.request_person_id).OnDelete(DeleteBehavior.NoAction);
            option.HasOne(x=>x.permission_person).WithMany(y=>y.given_access).HasForeignKey(x=>x.give_access_person_id).OnDelete(DeleteBehavior.SetNull);

        }
    }
}