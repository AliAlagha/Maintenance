using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class ReceiptItemConstraints : IEntityTypeConfiguration<ReceiptItem>
    {
        public void Configure(EntityTypeBuilder<ReceiptItem> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Customer).WithMany(x => x.ReceiptItems).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
