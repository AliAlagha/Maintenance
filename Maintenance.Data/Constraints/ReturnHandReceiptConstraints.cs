using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class ReturnHandReceiptConstraints : IEntityTypeConfiguration<ReturnHandReceipt>
    {
        public void Configure(EntityTypeBuilder<ReturnHandReceipt> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Customer).WithMany(x => x.ReturnHandReceipts).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Branch).WithMany(x => x.ReturnHandReceipts).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
