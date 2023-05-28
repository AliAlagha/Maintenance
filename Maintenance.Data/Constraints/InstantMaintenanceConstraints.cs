using Maintenance.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maintenance.Data.Constraints
{
    public class InstantMaintenanceConstraints : IEntityTypeConfiguration<InstantMaintenance>
    {
        public void Configure(EntityTypeBuilder<InstantMaintenance> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasOne(x => x.Technician).WithMany(x => x.InstantMaintenances).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Branch).WithMany(x => x.InstantMaintenances).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
