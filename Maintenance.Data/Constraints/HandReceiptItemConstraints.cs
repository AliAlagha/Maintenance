using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class HandReceiptItemConstraints : IEntityTypeConfiguration<HandReceiptItem>
    {
        public void Configure(EntityTypeBuilder<HandReceiptItem> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Customer).WithMany(x => x.HandReceiptItems).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Technician).WithMany(x => x.HandReceiptItems).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Branch).WithMany(x => x.HandReceiptItems).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
