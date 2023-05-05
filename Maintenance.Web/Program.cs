using AutoMapper;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.AutoMapper;
using Maintenance.Infrastructure.Extensions;
using Maintenance.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MaintenanceConnection");
builder.Services.AddDbContextPool<ApplicationDbContext>(opts =>
{
    opts.UseSqlServer(connectionString);
    opts.EnableDetailedErrors();
    opts.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<User, IdentityRole>(config =>
{
    config.User.RequireUniqueEmail = false; // to register a customer from another store
    config.Password.RequireDigit = false;
    config.Password.RequiredLength = 6;
    config.Password.RequireLowercase = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.SignIn.RequireConfirmedEmail = false;
    config.User.AllowedUserNameCharacters = null; // to accept digits and letters
})
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddSingleton(new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
}).CreateMapper());

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time   
});

builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    var supportedCulures = new List<CultureInfo> {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG"),//Arabic EG
        };
    opts.DefaultRequestCulture = new RequestCulture("ar-EG");
    opts.SupportedCultures = supportedCulures;
    opts.SupportedUICultures = supportedCulures;
});

builder.Services.AddAuthentication();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.RegisterServices();

// Middlewares
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

//Start => Language - Configure
var Options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(Options.Value);
//End => Language - Configure

app.UseExceptionHandler(opts => opts.UseMiddleware<ExceptionHandler>());

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.SeedDb();
app.Run();
