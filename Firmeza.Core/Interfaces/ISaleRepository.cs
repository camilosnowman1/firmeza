using Firmeza.Core.Entities;
using System.Collections.Generic;
using System.Linq; // Added for IQueryable
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int id);
    Task<IEnumerable<Sale>> GetAllAsync();
    IQueryable<Sale> GetAll(); // Added for pagination
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(int id);
}