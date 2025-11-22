using Firmeza.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public MigrationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Temporary endpoint to add ImageUrl columns to database
    /// DELETE THIS CONTROLLER AFTER USE!
    /// </summary>
    [HttpPost("add-imageurl-columns")]
    public async Task<IActionResult> AddImageUrlColumns()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Check if columns already exist
            var checkProductsSql = @"
                SELECT column_name 
                FROM information_schema.columns 
                WHERE table_name='Products' AND column_name='ImageUrl'";
            
            using var checkCmd = new NpgsqlCommand(checkProductsSql, connection);
            var productsHasColumn = await checkCmd.ExecuteScalarAsync() != null;

            if (!productsHasColumn)
            {
                var alterProductsSql = @"ALTER TABLE ""Products"" ADD COLUMN ""ImageUrl"" VARCHAR(500) NULL";
                using var cmd1 = new NpgsqlCommand(alterProductsSql, connection);
                await cmd1.ExecuteNonQueryAsync();
            }

            var checkVehiclesSql = @"
                SELECT column_name 
                FROM information_schema.columns 
                WHERE table_name='Vehicles' AND column_name='ImageUrl'";
            
            using var checkCmd2 = new NpgsqlCommand(checkVehiclesSql, connection);
            var vehiclesHasColumn = await checkCmd2.ExecuteScalarAsync() != null;

            if (!vehiclesHasColumn)
            {
                var alterVehiclesSql = @"ALTER TABLE ""Vehicles"" ADD COLUMN ""ImageUrl"" VARCHAR(500) NULL";
                using var cmd2 = new NpgsqlCommand(alterVehiclesSql, connection);
                await cmd2.ExecuteNonQueryAsync();
            }

            return Ok(new { 
                message = "ImageUrl columns added successfully",
                productsColumnAdded = !productsHasColumn,
                vehiclesColumnAdded = !vehiclesHasColumn
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to add columns", error = ex.Message });
        }
    }
}
