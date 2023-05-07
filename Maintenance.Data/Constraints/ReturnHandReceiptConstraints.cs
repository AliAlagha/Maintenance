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
        }
    }
}
