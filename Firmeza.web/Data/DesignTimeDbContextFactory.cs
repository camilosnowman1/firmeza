using Firmeza.Infrastructure.Persistence; // Corrected namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Firmeza.Web.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // This connection string is for design-time tools, not for runtime.
        optionsBuilder.UseNpgsql("Host=localhost;Database=firmeza_design;Username=postgres;Password=postgres");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}