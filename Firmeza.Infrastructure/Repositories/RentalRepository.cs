using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly ApplicationDbContext _context;

    public RentalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rental?> GetByIdAsync(int id)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public IQueryable<Rental> GetAll()
    {
        return _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .AsQueryable();
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .ToListAsync();
    }

    public async Task AddAsync(Rental rental)
    {
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Rental rental)
    {
        _context.Rentals.Update(rental);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rental = await GetByIdAsync(id);
        if (rental != null)
        {
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
        }
    }
}
