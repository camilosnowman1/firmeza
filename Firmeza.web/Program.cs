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

// Configure cookie settings to handle access denied redirects
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/Error";
    options.LoginPath = "/Account/Login";
});

// 3. Register Repositories and Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<SeedingService>();
builder.Services.AddTransient<Firmeza.Web.Data.SeedDb>();

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
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var seeder = services.GetRequiredService<SeedingService>();
    var seedDb = services.GetRequiredService<Firmeza.Web.Data.SeedDb>();

    try 
    {
        if (args.Contains("--reset-db"))
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Manual reset requested via --reset-db flag. Resetting database...");
            await ResetDatabaseAsync(context, seeder);
        }
        else
        {
            await context.Database.MigrateAsync();
            await seedDb.SeedAsync(); // Create admin user and roles first
            await seeder.SeedAsync(); // Then create products, vehicles, etc.
        }
    }
    catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07" || ex.SqlState == "42703")
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database schema mismatch detected (SqlState: {SqlState}). Resetting database...", ex.SqlState);
        await ResetDatabaseAsync(context, seeder);
    }
    catch (Exception ex)
    {
        var handled = false;
        var currentEx = ex;
        while (currentEx != null)
        {
            if (currentEx is Npgsql.PostgresException pgEx && (pgEx.SqlState == "42P07" || pgEx.SqlState == "42703"))
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(pgEx, "Database schema mismatch detected (SqlState: {SqlState}). Resetting database...", pgEx.SqlState);
                await ResetDatabaseAsync(context, seeder);
                handled = true;
                break;
            }
            currentEx = currentEx.InnerException;
        }

        if (!handled)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}

app.Run();

static async Task ResetDatabaseAsync(ApplicationDbContext context, SeedingService seeder)
{
    // If tables exist but migration fails, it means the DB is out of sync.
    // For this dev environment, we'll drop and recreate it.
    await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
    await seeder.SeedAsync();
}
