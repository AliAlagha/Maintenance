using Maintenance.Infrastructure.Services.Colors;
using Maintenance.Infrastructure.Services.Companies;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.Files;
using Maintenance.Infrastructure.Services.Items;
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
            return services;
        }
    }
}
