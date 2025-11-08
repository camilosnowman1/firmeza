using Firmeza.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Firmeza.Core.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);
    IQueryable<Vehicle> GetAll();
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(int id);
    Task<IEnumerable<Vehicle>> GetAllAsync(); // Keep this for cases where all vehicles are needed in memory
}