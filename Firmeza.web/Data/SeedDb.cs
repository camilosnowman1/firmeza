using Firmeza.Core.Entities; // Corrected namespace
using Firmeza.Infrastructure.Persistence; // Corrected namespace
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        await _context.Database.MigrateAsync();

        // Create roles
        await CheckRoleAsync("Admin");
        await CheckRoleAsync("Cliente");

        // Create admin user
        await CheckUserAsync("admin@firmeza.dev", "Admin");

        // Create products if they don't exist
        if (!_context.Products.Any())
        {
            _context.Products.Add(new Product { Name = "Cement", Price = 10.50m });
            _context.Products.Add(new Product { Name = "Sand Bag", Price = 5.25m });
            _context.Products.Add(new Product { Name = "Bricks (x100)", Price = 25.00m });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRoleAsync(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private async Task CheckUserAsync(string email, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}