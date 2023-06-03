using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class RecipientMaintenanceConstraints : IEntityTypeConfiguration<RecipientMaintenance>
    {
        public void Configure(EntityTypeBuilder<RecipientMaintenance> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Branch).WithMany(x => x.RecipientMaintenances).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        }
    }
}
