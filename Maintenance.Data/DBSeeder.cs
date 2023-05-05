using Maintenance.Core.Constants;
using Maintenance.Core.Enums;
using Maintenance.Core.Exceptions;
using Maintenance.Data.DbEntities;
using Maintenance.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Maintenance.Data
{
    public static class DBSeeder
    {
        public static IHost SeedDb(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    roleManager.SeedRoles().Wait();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    userManager.SeedUsers(context).Wait();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    //throw;
                }
            }
            return webHost;
        }

        public static async Task SeedRoles(this RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.Roles.AnyAsync()) return;

            await roleManager.CreateAsync(new IdentityRole(RoleNames.Administrator));
            await roleManager.CreateAsync(new IdentityRole(RoleNames.MaintenanceManager));
            await roleManager.CreateAsync(new IdentityRole(RoleNames.MaintenanceTechnician));
        }

        public static async Task SeedUsers(this UserManager<User> userManager, ApplicationDbContext context)
        {
            await TransactionExtension.UseTransaction(context, async () =>
            {
                if (await userManager.Users.AnyAsync()) return;

                var users = new List<User>
            {
                new User
                {
                    FullName = "System Administrator",
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    UserType = UserType.Administrator,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },
                new User
                {
                    FullName = "Maintenance Manager",
                    UserName = "manager@test.com",
                    Email = "manager@test.com",
                    UserType = UserType.MaintenanceManager,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                },
                new User
                {
                    FullName = "Maintenance Technician",
                    UserName = "technician@test.com",
                    Email = "technician@test.com",
                    UserType = UserType.MaintenanceTechnician,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }
            };

                foreach (var user in users)
                {
                    var createUserResult = await userManager.CreateAsync(user, "123321");
                    if (!createUserResult.Succeeded)
                    {
                        throw new OperationFailedException();
                    }

                    await AddUserToRole(userManager, user);
                }
            });
        }

        private static async Task AddUserToRole(UserManager<User> userManager, User user)
        {
            IdentityResult? addToRoleResult = null;
            switch (user.UserType)
            {
                case UserType.Administrator:
                    addToRoleResult = await userManager.AddToRoleAsync(user, RoleNames.Administrator);
                    break;
                case UserType.MaintenanceManager:
                    addToRoleResult = await userManager.AddToRoleAsync(user, RoleNames.MaintenanceManager);
                    break;
                case UserType.MaintenanceTechnician:
                    addToRoleResult = await userManager.AddToRoleAsync(user, RoleNames.MaintenanceTechnician);
                    break;
            }

            if (addToRoleResult == null || !addToRoleResult.Succeeded)
            {
                throw new OperationFailedException();
            }
        }

    }
}