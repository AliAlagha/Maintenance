using Maintenance.Infrastructure.Services.Branches;
using Maintenance.Infrastructure.Services.Colors;
using Maintenance.Infrastructure.Services.Companies;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.Files;
using Maintenance.Infrastructure.Services.HandReceiptItems;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Items;
using Maintenance.Infrastructure.Services.Maintenance;
using Maintenance.Infrastructure.Services.ReturnHandReceiptItems;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Maintenance.Infrastructure.Extensions
{
    public static class ContainerRegistryExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IColorService, ColorService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IHandReceiptService, HandReceiptService>();
            services.AddScoped<IReturnHandReceiptService, ReturnHandReceiptService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IHandReceiptItemService, HandReceiptItemService>();
            services.AddScoped<IReturnHandReceiptItemService, ReturnHandReceiptItemService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            return services;
        }
    }
}
