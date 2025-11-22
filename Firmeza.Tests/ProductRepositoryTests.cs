using Xunit;
using Firmeza.Core.Entities;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Tests;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test class instance
            .Options;
        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);

        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Price = 100, Stock = 10 };

        // Act
        await _repository.AddAsync(product);

        // Assert
        var savedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Product", savedProduct.Name);
        Assert.Equal(100, savedProduct.Price);
        Assert.Equal(10, savedProduct.Stock);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Name = "Existing Product", Price = 50, Stock = 5 };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var fetchedProduct = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(fetchedProduct);
        Assert.Equal(product.Name, fetchedProduct.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllProducts()
    {
        // Arrange
        await _context.Products.AddAsync(new Product { Name = "Product 1", Price = 10, Stock = 1 });
        await _context.Products.AddAsync(new Product { Name = "Product 2", Price = 20, Stock = 2 });
        await _context.SaveChangesAsync();

        // Act
        var products = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product { Name = "Original Name", Price = 10, Stock = 1 };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        product.Name = "Updated Name";
        product.Price = 15;

        // Act
        await _repository.UpdateAsync(product);

        // Assert
        var updatedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Name", updatedProduct.Name);
        Assert.Equal(15, updatedProduct.Price);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Arrange
        var product = new Product { Name = "Product to Delete", Price = 10, Stock = 1 };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(product.Id);

        // Assert
        var deletedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.Null(deletedProduct);
    }
}
