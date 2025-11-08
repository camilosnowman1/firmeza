using Firmeza.Core.Entities;

namespace Firmeza.Core.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    IQueryable<Customer> GetAll(); // Changed from Task<IEnumerable<Customer>> GetAllAsync()
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    Task<IEnumerable<Customer>> GetAllAsync();
}