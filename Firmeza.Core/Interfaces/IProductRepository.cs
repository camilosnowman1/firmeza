using Firmeza.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Firmeza.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    IQueryable<Product> GetAll(); // Changed from Task<IEnumerable<Product>> GetAllAsync()
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync(); // Keep this for cases where all products are needed in memory
}