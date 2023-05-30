using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class HandReceiptConstraints : IEntityTypeConfiguration<HandReceipt>
    {
        public void Configure(EntityTypeBuilder<HandReceipt> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Customer).WithMany(x => x.HandReceipts).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Branch).WithMany(x => x.HandReceipts).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
