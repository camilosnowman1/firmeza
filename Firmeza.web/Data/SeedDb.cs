using Firmeza.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Web.Data;

public class SeedDb
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedDb(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        
        // 1. Create roles
        await CheckRolesAsync();
        
        // 2. Create a default admin user
        await CheckUsersAsync();
    }

    private async Task CheckRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await _roleManager.RoleExistsAsync("Customer"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Customer"));
        }
    }

    private async Task CheckUsersAsync()
    {
        // Create Admin user
        var adminUser = await _userManager.FindByEmailAsync("admin@firmeza.dev");
        if (adminUser == null)
        { 
            adminUser = new ApplicationUser
            {
                UserName = "admin@firmeza.dev",
                Email = "admin@firmeza.dev",
            };
            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}