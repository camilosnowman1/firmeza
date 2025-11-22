using System.Globalization;
using Firmeza.Infrastructure.Services;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

// 1. Configure QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// 2. Configure the database context and Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, 
        b => b.MigrationsAssembly("Firmeza.Infrastructure")));

// Modified to include AddRoles and AddRoleManager
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>();

// 3. Register Repositories and Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<SeedingService>();

var app = builder.Build();

// --- Configure Colombian Culture ---
var supportedCultures = new[] { new CultureInfo("es-CO") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-CO"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});
// --- End of Culture Configuration ---

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedingService>();
    await seeder.SeedAsync();
}
app.Run();
