using System.Globalization;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Repositories;
using Firmeza.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

// 1. Configure QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// 2. Configure the database context and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        b => b.MigrationsAssembly("Firmeza.Infrastructure")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Register Repositories and Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>(); // Added this missing line
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

// Removed the problematic custom model binder configuration.
// The RequestLocalizationOptions should handle decimal parsing based on culture.
builder.Services.AddControllersWithViews();


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

app.Run();
