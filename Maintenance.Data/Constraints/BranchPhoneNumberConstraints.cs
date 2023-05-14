using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class BranchPhoneNumberConstraints : IEntityTypeConfiguration<BranchPhoneNumber>
    {
        public void Configure(EntityTypeBuilder<BranchPhoneNumber> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }
}
