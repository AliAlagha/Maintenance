using Maintenance.Data.Constraints;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Data.Extensions
{
    public static class DbConstraintsExtension
    {
        public static ModelBuilder ApplyConstraints(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConstraints());
            return modelBuilder;
        }
    }
}
