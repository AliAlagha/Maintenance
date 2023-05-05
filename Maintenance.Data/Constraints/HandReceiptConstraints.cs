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
        }
    }
}
