using Firmeza.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int id);
    Task<IEnumerable<Sale>> GetAllAsync();
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(int id);
}