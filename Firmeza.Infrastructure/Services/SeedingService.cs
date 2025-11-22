using Firmeza.Core.Entities;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Firmeza.Infrastructure.Services;

public class SeedingService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var productRepository = scope.ServiceProvider.GetRequiredService<Core.Interfaces.IProductRepository>();

        // 1. Seed Products
        if (await context.Products.CountAsync() < 50)
        {
            var productNames = new[] { "Taladro", "Sierra", "Martillo", "Destornillador", "Llave Inglesa", "Lijadora", "Pulidora", "Compresor" };
            var adjectives = new[] { "Industrial", "Profesional", "Hogar", "Inalámbrico", "Potente", "Compacto" };
            
            for (int i = 0; i < 50; i++)
            {
                var name = $"{productNames[new Random().Next(productNames.Length)]} {adjectives[new Random().Next(adjectives.Length)]} {i}";
                if (!await context.Products.AnyAsync(p => p.Name == name))
                {
                    await context.Products.AddAsync(new Product
                    {
                        Name = name,
                        Description = $"Descripción del producto {name}",
                        Price = new Random().Next(50, 500),
                        Stock = new Random().Next(1, 100),
                        ImageUrl = "https://via.placeholder.com/400",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        // 2. Seed Vehicles
        if (await context.Vehicles.CountAsync() < 50)
        {
            var vehicleTypes = new[] { "Excavadora", "Retroexcavadora", "Grúa", "Camión Volquete", "Aplanadora", "Cargador Frontal", "Montacargas", "Mezcladora" };
            var brands = new[] { "CAT", "JCB", "Komatsu", "Volvo", "John Deere", "Hitachi", "Liebherr" };
            
            for (int i = 1; i <= 50; i++)
            {
                var type = vehicleTypes[new Random().Next(vehicleTypes.Length)];
                var brand = brands[new Random().Next(brands.Length)];
                var model = new Random().Next(100, 900);
                var name = $"{type} {brand} {model}";
                
                if (!await context.Vehicles.AnyAsync(v => v.Name == name))
                {
                    context.Vehicles.Add(new Vehicle
                    {
                        Name = name,
                        Description = $"Maquinaria pesada {brand} modelo {model}, año 202{new Random().Next(0, 5)}",
                        HourlyRate = new Random().Next(50, 500),
                        ImageUrl = "https://via.placeholder.com/400",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        // 3. Seed Customers
        if (await context.Customers.CountAsync() < 100)
        {
            for (int i = 1; i <= 100; i++)
            {
                var doc = $"900{i:000000}";
                if (!await context.Customers.AnyAsync(c => c.Document == doc))
                {
                    context.Customers.Add(new Customer
                    {
                        FullName = $"Cliente Generado {i}",
                        Document = doc,
                        Email = $"cliente{i}@ejemplo.com",
                        PhoneNumber = $"300{new Random().Next(1000000, 9999999)}",
                        Address = $"Calle {new Random().Next(1, 100)} # {new Random().Next(1, 100)} - {new Random().Next(1, 100)}"
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        // 4. Seed Rentals
        if (await context.Rentals.CountAsync() < 100)
        {
            var customers = await context.Customers.ToListAsync();
            var vehicles = await context.Vehicles.ToListAsync();
            var random = new Random();

            if (customers.Any() && vehicles.Any())
            {
                for (int i = 0; i < 100; i++)
                {
                    var customer = customers[random.Next(customers.Count)];
                    var vehicle = vehicles[random.Next(vehicles.Count)];
                    var start = DateTime.UtcNow.AddDays(-random.Next(1, 365));
                    var end = start.AddHours(random.Next(4, 72));

                    context.Rentals.Add(new Rental
                    {
                        CustomerId = customer.Id,
                        VehicleId = vehicle.Id,
                        StartDate = start,
                        EndDate = end,
                        TotalAmount = (decimal)(end - start).TotalHours * vehicle.HourlyRate,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        // 5. Seed Sales
        if (await context.Sales.CountAsync() < 100)
        {
            var customers = await context.Customers.ToListAsync();
            var products = await productRepository.GetAllAsync();
            var productList = products.ToList();
            
            if (customers.Any() && productList.Any())
            {
                var random = new Random();

                for (int i = 0; i < 100; i++)
                {
                    var customer = customers[random.Next(customers.Count)];
                    var saleDetails = new List<SaleDetail>();
                    var numItems = random.Next(1, 5);
                    decimal total = 0;

                    for (int j = 0; j < numItems; j++)
                    {
                        var prod = productList[random.Next(productList.Count)];
                        var qty = random.Next(1, 5);
                        saleDetails.Add(new SaleDetail { ProductId = prod.Id, Quantity = qty, UnitPrice = prod.Price, TotalPrice = prod.Price * qty });
                        total += prod.Price * qty;
                    }

                    var sale = new Sale
                    {
                        CustomerId = customer.Id,
                        SaleDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                        SaleDetails = saleDetails,
                        TotalAmount = total
                    };
                    
                    context.Sales.Add(sale);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
