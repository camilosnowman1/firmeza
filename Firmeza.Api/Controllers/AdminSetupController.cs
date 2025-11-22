using Firmeza.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminSetupController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminSetupController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Temporary endpoint to add Admin role to camilosnowman@gmail.com
    /// DELETE THIS CONTROLLER AFTER USE!
    /// </summary>
    [HttpPost("add-admin-role")]
    public async Task<IActionResult> AddAdminRole()
    {
        var email = "camilosnowman@gmail.com";
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return NotFound(new { message = $"User {email} not found" });
        }

        // Ensure Admin role exists
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Check if user already has Admin role
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return Ok(new { message = $"User {email} already has Admin role" });
        }

        // Add Admin role to user
        var result = await _userManager.AddToRoleAsync(user, "Admin");
        
        if (result.Succeeded)
        {
            return Ok(new { message = $"Admin role successfully added to {email}" });
        }

        return BadRequest(new { message = "Failed to add Admin role", errors = result.Errors });
    }

    /// <summary>
    /// Temporary endpoint to reset password for camilosnowman@gmail.com
    /// DELETE THIS CONTROLLER AFTER USE!
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword()
    {
        var email = "camilosnowman@gmail.com";
        var newPassword = "Camilosnow123#";
        
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return NotFound(new { message = $"User {email} not found" });
        }

        // Remove existing password and set new one
        await _userManager.RemovePasswordAsync(user);
        var result = await _userManager.AddPasswordAsync(user, newPassword);
        
        if (result.Succeeded)
        {
            return Ok(new { message = $"Password reset successfully for {email}", newPassword });
        }

        return BadRequest(new { message = "Failed to reset password", errors = result.Errors });
    }
}
