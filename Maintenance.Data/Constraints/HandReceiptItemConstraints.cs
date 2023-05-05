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
        }
    }
}
