using Maintenance.Data.Constraints;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Data.Extensions
{
    public static class DbConstraintsExtension
    {
        public static ModelBuilder ApplyConstraints(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConstraints());
            modelBuilder.ApplyConfiguration(new ColorConstraints());
            modelBuilder.ApplyConfiguration(new CompanyConstraints());
            modelBuilder.ApplyConfiguration(new CustomerConstraints());
            modelBuilder.ApplyConfiguration(new ItemConstraints());
            modelBuilder.ApplyConfiguration(new HandReceiptConstraints());
            modelBuilder.ApplyConfiguration(new HandReceiptItemConstraints());
            modelBuilder.ApplyConfiguration(new ReturnHandReceiptConstraints());
            modelBuilder.ApplyConfiguration(new ReturnHandReceiptItemConstraints());
            modelBuilder.ApplyConfiguration(new RecipientMaintenanceConstraints());
            modelBuilder.ApplyConfiguration(new BranchConstraints());
            return modelBuilder;
        }
    }
}
