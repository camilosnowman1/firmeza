using Xunit;
using Firmeza.Web.Helpers;
using Firmeza.Core.Entities;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Tests.Helpers;

public class PaginatedListTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public PaginatedListTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        // Seed data
        var products = Enumerable.Range(1, 100).Select(i => new Product 
        { 
            Name = $"Product {i}", 
            Price = i, 
            Stock = 10 
        });
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 3; // Requesting the 3rd page

        // Act
        var paginatedList = await PaginatedList<Product>.CreateAsync(_context.Products.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.NotNull(paginatedList);
        Assert.Equal(pageIndex, paginatedList.PageIndex);
        Assert.Equal(10, paginatedList.Count); // Should contain 10 items
        Assert.Equal("Product 21", paginatedList[0].Name); // First item on 3rd page (0-indexed skip)
        Assert.Equal(10, paginatedList.TotalPages);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleFirstPage()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 1;

        // Act
        var paginatedList = await PaginatedList<Product>.CreateAsync(_context.Products.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.NotNull(paginatedList);
        Assert.Equal(pageIndex, paginatedList.PageIndex);
        Assert.Equal(10, paginatedList.Count);
        Assert.Equal("Product 1", paginatedList[0].Name);
        Assert.True(paginatedList.HasNextPage);
        Assert.False(paginatedList.HasPreviousPage);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleLastPage()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 10; // Last page

        // Act
        var paginatedList = await PaginatedList<Product>.CreateAsync(_context.Products.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.NotNull(paginatedList);
        Assert.Equal(pageIndex, paginatedList.PageIndex);
        Assert.Equal(10, paginatedList.Count);
        Assert.Equal("Product 91", paginatedList[0].Name); // First item on last page
        Assert.False(paginatedList.HasNextPage);
        Assert.True(paginatedList.HasPreviousPage);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleEmptySource()
    {
        // Arrange
        // Create a separate empty context or just filter to empty
        var emptyQuery = _context.Products.Where(p => p.Name == "NonExistent");
        var pageSize = 10;
        var pageIndex = 1;

        // Act
        var paginatedList = await PaginatedList<Product>.CreateAsync(emptyQuery, pageIndex, pageSize);

        // Assert
        Assert.NotNull(paginatedList);
        Assert.Equal(0, paginatedList.Count);
        Assert.Equal(0, paginatedList.TotalPages);
        Assert.False(paginatedList.HasNextPage);
        Assert.False(paginatedList.HasPreviousPage);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandlePageOutOfRange()
    {
        // Arrange
        var pageSize = 10;
        var pageIndex = 15; // Page beyond total pages

        // Act
        var paginatedList = await PaginatedList<Product>.CreateAsync(_context.Products.AsQueryable(), pageIndex, pageSize);

        // Assert
        Assert.NotNull(paginatedList);
        Assert.Equal(10, paginatedList.TotalPages); // Still 10 total pages
        Assert.Equal(0, paginatedList.Count); // No items on this page
    }
}
