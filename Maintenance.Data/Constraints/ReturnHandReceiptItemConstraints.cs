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
        }
    }
}
