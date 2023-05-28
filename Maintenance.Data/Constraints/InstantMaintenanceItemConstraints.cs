using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class InstantMaintenanceItemConstraints : IEntityTypeConfiguration<InstantMaintenanceItem>
    {
        public void Configure(EntityTypeBuilder<InstantMaintenanceItem> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Technician).WithMany(x => x.InstantMaintenanceItems).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            builder.HasOne(x => x.Branch).WithMany(x => x.InstantMaintenanceItems).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
