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

        // 1. Seed Products - Productos de Ferretería
        if (await context.Products.CountAsync() < 50)
        {
            var ferreteriaProducts = new[]
            {
                // Herramientas Eléctricas
                new { Name = "Taladro Percutor 750W", Desc = "Taladro percutor profesional con velocidad variable y reversible", Price = 189000m, Stock = 15 },
                new { Name = "Sierra Circular 7 1/4\"", Desc = "Sierra circular eléctrica 1400W con guía láser", Price = 245000m, Stock = 12 },
                new { Name = "Pulidora Angular 4 1/2\"", Desc = "Pulidora angular 850W ideal para corte y desbaste", Price = 135000m, Stock = 20 },
                new { Name = "Lijadora Orbital", Desc = "Lijadora orbital 300W con sistema de recolección de polvo", Price = 165000m, Stock = 10 },
                new { Name = "Taladro Inalámbrico 20V", Desc = "Taladro atornillador inalámbrico con batería de litio", Price = 285000m, Stock = 8 },
                new { Name = "Compresor de Aire 50L", Desc = "Compresor de aire 2HP ideal para pintura y herramientas neumáticas", Price = 890000m, Stock = 5 },
                
                // Herramientas Manuales
                new { Name = "Juego de Llaves Combinadas 12 Pzs", Desc = "Set de llaves combinadas métricas de 8mm a 19mm", Price = 125000m, Stock = 25 },
                new { Name = "Martillo de Carpintero 16oz", Desc = "Martillo con mango de fibra de vidrio antideslizante", Price = 35000m, Stock = 30 },
                new { Name = "Destornilladores Set 6 Pzs", Desc = "Juego de destornilladores planos y phillips profesionales", Price = 45000m, Stock = 40 },
                new { Name = "Alicate Universal 8\"", Desc = "Alicate universal con empuñadura ergonómica", Price = 28000m, Stock = 35 },
                new { Name = "Nivel de Burbuja 24\"", Desc = "Nivel de aluminio con 3 burbujas de precisión", Price = 42000m, Stock = 18 },
                new { Name = "Flexómetro 5m", Desc = "Cinta métrica de 5 metros con freno automático", Price = 18000m, Stock = 50 },
                
                // Materiales de Construcción
                new { Name = "Cemento Gris 50kg", Desc = "Bulto de cemento portland gris uso general", Price = 28500m, Stock = 200 },
                new { Name = "Arena Lavada m³", Desc = "Metro cúbico de arena lavada para construcción", Price = 85000m, Stock = 50 },
                new { Name = "Gravilla m³", Desc = "Metro cúbico de gravilla para concreto", Price = 95000m, Stock = 40 },
                new { Name = "Ladrillo Tolete x 100", Desc = "Paquete de 100 ladrillos tolete para mampostería", Price = 185000m, Stock = 80 },
                new { Name = "Bloque de Cemento #4", Desc = "Bloque hueco de cemento 10x20x40cm", Price = 2800m, Stock = 500 },
                new { Name = "Varilla Corrugada 3/8\" x 6m", Desc = "Varilla de acero corrugada para refuerzo", Price = 32000m, Stock = 150 },
                
                // Pinturas y Acabados
                new { Name = "Pintura Vinilo Blanco 5 Galones", Desc = "Pintura vinilo lavable para interiores", Price = 165000m, Stock = 30 },
                new { Name = "Esmalte Sintético Galón", Desc = "Esmalte sintético brillante para madera y metal", Price = 58000m, Stock = 45 },
                new { Name = "Estuco Plástico 25kg", Desc = "Estuco plástico para alisar paredes interiores", Price = 42000m, Stock = 60 },
                new { Name = "Rodillo + Bandeja", Desc = "Kit de rodillo de 9\" con bandeja plástica", Price = 22000m, Stock = 35 },
                new { Name = "Brochas Set 3 Pzs", Desc = "Juego de brochas 1\", 2\" y 3\" cerda sintética", Price = 28000m, Stock = 40 },
                
                // Plomería
                new { Name = "Tubo PVC 4\" x 6m", Desc = "Tubo PVC sanitario 4 pulgadas x 6 metros", Price = 48000m, Stock = 70 },
                new { Name = "Codo PVC 90° 4\"", Desc = "Codo PVC sanitario 90 grados 4 pulgadas", Price = 8500m, Stock = 100 },
                new { Name = "Llave de Paso 1/2\"", Desc = "Llave de paso esférica de bronce 1/2 pulgada", Price = 18000m, Stock = 50 },
                new { Name = "Sifón Lavamanos", Desc = "Sifón cromado ajustable para lavamanos", Price = 15000m, Stock = 30 },
                new { Name = "Flexible Agua 1/2\" x 30cm", Desc = "Manguera flexible trenzada para agua", Price = 12000m, Stock = 60 },
                
                // Electricidad
                new { Name = "Cable THHN #12 x 100m", Desc = "Rollo de cable eléctrico calibre 12 AWG", Price = 185000m, Stock = 25 },
                new { Name = "Tomacorriente Doble", Desc = "Tomacorriente doble polarizado 15A", Price = 8500m, Stock = 80 },
                new { Name = "Interruptor Sencillo", Desc = "Interruptor sencillo 15A color blanco", Price = 6500m, Stock = 100 },
                new { Name = "Caja Octogonal Metálica", Desc = "Caja octogonal galvanizada para techo", Price = 4500m, Stock = 120 },
                new { Name = "Bombillo LED 12W", Desc = "Bombillo LED luz blanca 12W equivalente 100W", Price = 15000m, Stock = 90 },
                
                // Seguridad y Protección
                new { Name = "Casco de Seguridad", Desc = "Casco de seguridad industrial con ajuste de matraca", Price = 28000m, Stock = 40 },
                new { Name = "Guantes de Carnaza", Desc = "Par de guantes de carnaza reforzados", Price = 12000m, Stock = 60 },
                new { Name = "Gafas de Seguridad", Desc = "Gafas de seguridad transparentes antiempañantes", Price = 8500m, Stock = 70 },
                new { Name = "Tapabocas N95 x 10", Desc = "Paquete de 10 tapabocas N95 certificados", Price = 35000m, Stock = 50 },
                
                // Adhesivos y Sellantes
                new { Name = "Pegante para Cerámica 25kg", Desc = "Pegante adhesivo para cerámica y porcelanato", Price = 38000m, Stock = 45 },
                new { Name = "Silicona Transparente", Desc = "Silicona sellante transparente cartucho 300ml", Price = 12000m, Stock = 80 },
                new { Name = "Pegante PVC 1/4 Galón", Desc = "Soldadura líquida para tubería PVC", Price = 28000m, Stock = 55 },
                new { Name = "Espuma Poliuretano", Desc = "Espuma expansiva de poliuretano en aerosol", Price = 22000m, Stock = 40 },
                
                // Ferretería General
                new { Name = "Tornillos Drywall x 500", Desc = "Caja de 500 tornillos para drywall punta fina", Price = 18000m, Stock = 60 },
                new { Name = "Puntillas 2\" x 1kg", Desc = "Kilogramo de puntillas de acero 2 pulgadas", Price = 8500m, Stock = 100 },
                new { Name = "Candado de Seguridad 50mm", Desc = "Candado de seguridad laminado 50mm con 3 llaves", Price = 35000m, Stock = 45 },
                new { Name = "Bisagra 3\" Par", Desc = "Par de bisagras de acero 3 pulgadas", Price = 12000m, Stock = 70 }
            };

            foreach (var product in ferreteriaProducts)
            {
                if (!await context.Products.AnyAsync(p => p.Name == product.Name))
                {
                    await context.Products.AddAsync(new Product
                    {
                        Name = product.Name,
                        Description = product.Desc,
                        Price = product.Price,
                        Stock = product.Stock,
                        ImageUrl = "https://via.placeholder.com/400x300/4CAF50/FFFFFF?text=" + Uri.EscapeDataString(product.Name.Split(' ')[0]),
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
