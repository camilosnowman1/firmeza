using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public IQueryable<Vehicle> GetAll()
    {
        return _context.Vehicles.AsQueryable();
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles.ToListAsync();
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var vehicle = await GetByIdAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
