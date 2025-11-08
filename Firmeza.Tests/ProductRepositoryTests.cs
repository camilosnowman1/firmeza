using NUnit.Framework;
using Firmeza.Core.Entities;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Tests;

[TestFixture]
public class ProductRepositoryTests
{
    private ApplicationDbContext _context;
    private ProductRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);

        // Clear the database before each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Price = 100, Stock = 10 };

        // Act
        await _repository.AddAsync(product);

        // Assert
        var savedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
        Assert.IsNotNull(savedProduct);
        Assert.AreEqual("Test Product", savedProduct.Name);
        Assert.AreEqual(100, savedProduct.Price);
        Assert.AreEqual(10, savedProduct.Stock);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product { Name = "Existing Product", Price = 50, Stock = 5 };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var fetchedProduct = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.IsNotNull(fetchedProduct);
        Assert.AreEqual(product.Name, fetchedProduct.Name);
    }

    [Test]
    public async Task GetAll_ShouldReturnAllProducts()
    {
        // Arrange
        await _context.Products.AddAsync(new Product { Name = "Product 1", Price = 10, Stock = 1 });
        await _context.Products.AddAsync(new Product { Name = "Product 2", Price = 20, Stock = 2 });
        await _context.SaveChangesAsync();

        // Act
        var products = _repository.GetAll().ToList();

        // Assert
        Assert.AreEqual(2, products.Count);
    }

    [Test]
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
        Assert.IsNotNull(updatedProduct);
        Assert.AreEqual("Updated Name", updatedProduct.Name);
        Assert.AreEqual(15, updatedProduct.Price);
    }

    [Test]
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
        Assert.IsNull(deletedProduct);
    }
}
