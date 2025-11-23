using Firmeza.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MigrationController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// TEMPORARY: Clean all products and vehicles to allow reseeding
    /// DELETE THIS ENDPOINT AFTER USE!
    /// </summary>
    [HttpPost("clean-products")]
    public async Task<IActionResult> CleanProducts()
    {
        try
        {
            // Delete in correct order due to foreign keys
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"SaleDetails\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Sales\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Rentals\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Products\"");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Vehicles\"");
            
            return Ok(new { 
                message = "Database cleaned successfully. Restart the application to reseed with ferretería products.",
                deleted = new {
                    saleDetails = "all",
                    sales = "all",
                    rentals = "all",
                    products = "all",
                    vehicles = "all"
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error cleaning database", error = ex.Message });
        }
    }
}
