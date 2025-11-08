using Firmeza.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Firmeza.Core.Interfaces;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(int id);
    IQueryable<Rental> GetAll();
    Task AddAsync(Rental rental);
    Task UpdateAsync(Rental rental);
    Task DeleteAsync(int id);
    Task<IEnumerable<Rental>> GetAllAsync(); // Keep this for cases where all rentals are needed in memory
}