using Xunit;
using Firmeza.Web.Controllers;
using Firmeza.Core.Entities;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Firmeza.Tests.Controllers;

public class ImportControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _productRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly ImportController _controller;

    public ImportControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _productRepository = new ProductRepository(_context);
        _customerRepository = new CustomerRepository(_context);

        _controller = new ImportController(_productRepository, _customerRepository, _context);
        
        // Mock TempData
        var tempData = new Mock<ITempDataDictionary>();
        _controller.TempData = tempData.Object;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private IFormFile CreateExcelFile(string sheetName, List<string[]> rows)
    {
        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Length; c++)
                {
                    worksheet.Cells[r + 1, c + 1].Value = rows[r][c];
                }
            }
            package.Save();
        }
        stream.Position = 0;

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((target, token) => stream.CopyTo(target));

        return fileMock.Object;
    }

    [Fact]
    public async Task UploadProducts_ShouldImportValidProducts()
    {
        // Arrange
        var rows = new List<string[]>
        {
            new[] { "Product Name", "Price" },
            new[] { "Cement", "25.50" },
            new[] { "Bricks", "0.50" }
        };
        var file = CreateExcelFile("Products", rows);

        // Act
        var result = await _controller.UploadProducts(file);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Products", redirectResult.ControllerName);

        var products = await _context.Products.ToListAsync();
        Assert.Equal(2, products.Count);
        Assert.Contains(products, p => p.Name == "Cement" && p.Price == 25.50m);
        Assert.Contains(products, p => p.Name == "Bricks" && p.Price == 0.50m);
    }

    [Fact]
    public async Task UploadCustomers_ShouldImportValidCustomers()
    {
        // Arrange
        var rows = new List<string[]>
        {
            new[] { "Document", "FullName", "Email", "PhoneNumber", "Address" },
            new[] { "12345", "John Doe", "john@example.com", "555-1234", "123 Main St" }
        };
        var file = CreateExcelFile("Customers", rows);

        // Act
        var result = await _controller.UploadCustomers(file);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Customers", redirectResult.ControllerName);

        var customers = await _context.Customers.ToListAsync();
        Assert.Single(customers);
        var customer = customers.First();
        Assert.Equal("12345", customer.Document);
        Assert.Equal("John Doe", customer.FullName);
        Assert.Equal("john@example.com", customer.Email);
    }
}
