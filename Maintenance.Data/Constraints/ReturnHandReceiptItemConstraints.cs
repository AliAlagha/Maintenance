using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class ReturnHandReceiptItemConstraints : IEntityTypeConfiguration<ReturnHandReceiptItem>
    {
        public void Configure(EntityTypeBuilder<ReturnHandReceiptItem> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Customer).WithMany(x => x.ReturnHandReceiptItems).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Technician).WithMany(x => x.ReturnHandReceiptItems).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Branch).WithMany(x => x.ReturnHandReceiptItems).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ReturnHandReceipt).WithMany(x => x.ReturnHandReceiptItems).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
