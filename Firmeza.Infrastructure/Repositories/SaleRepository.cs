using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;

    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
            .ToListAsync();
    }

    public async Task AddAsync(Sale sale)
    {
        // Calculate total amount and ensure product prices are correct
        foreach (var detail in sale.SaleDetails)
        {
            var product = await _context.Products.FindAsync(detail.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {detail.ProductId} not found.");
            }
            detail.UnitPrice = product.Price; // Ensure unit price is current product price
            sale.TotalAmount += detail.TotalPrice; // TotalPrice is calculated in SaleDetail entity
        }

        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Sale sale)
    {
        // This is a complex operation for sales with details. 
        // For simplicity, we'll assume sale.SaleDetails contains the desired state.
        // A more robust solution would involve comparing existing details with new ones.

        var existingSale = await _context.Sales
            .Include(s => s.SaleDetails)
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        if (existingSale == null)
        {
            throw new InvalidOperationException($"Sale with ID {sale.Id} not found.");
        }

        _context.Entry(existingSale).CurrentValues.SetValues(sale);
        existingSale.SaleDetails.Clear(); // Remove old details
        existingSale.TotalAmount = 0;

        foreach (var newDetail in sale.SaleDetails)
        {
            var product = await _context.Products.FindAsync(newDetail.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {newDetail.ProductId} not found.");
            }
            newDetail.UnitPrice = product.Price; // Ensure unit price is current product price
            existingSale.SaleDetails.Add(newDetail);
            existingSale.TotalAmount += newDetail.TotalPrice;
        }

        _context.Sales.Update(existingSale);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sale = await GetByIdAsync(id);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }
}