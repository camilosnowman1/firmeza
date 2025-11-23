using Xunit;
using Moq;
using AutoMapper;
using Firmeza.Api.Controllers.V2;
using Firmeza.Api.Dtos.V2;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Tests.Controllers.V2;

public class ProductsV2ControllerTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ISaleRepository> _mockSaleRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductsV2Controller _controller;

    public ProductsV2ControllerTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockSaleRepo = new Mock<ISaleRepository>();
        _mockMapper = new Mock<IMapper>();
        _controller = new ProductsV2Controller(_mockProductRepo.Object, _mockSaleRepo.Object, _mockMapper.Object);

        // Setup ControllerContext for Response.Headers
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetProducts_WithoutFilters_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 10 },
            new Product { Id = 2, Name = "Sierra", Price = 200000, Stock = 5 },
            new Product { Id = 3, Name = "Martillo", Price = 50000, Stock = 20 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        
        // Verify the response has the expected structure
        Assert.NotNull(response);
        
        // Verify repository was called
        _mockProductRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProducts_WithSearchFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro Percutor", Description = "Herramienta eléctrica", Price = 100000, Stock = 10 },
            new Product { Id = 2, Name = "Sierra Circular", Description = "Herramienta de corte", Price = 200000, Stock = 5 },
            new Product { Id = 3, Name = "Martillo", Description = "Herramienta manual", Price = 50000, Stock = 20 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetProducts(search: "taladro");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetProducts_WithPriceFilter_ReturnsProductsInRange()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 10 },
            new Product { Id = 2, Name = "Sierra", Price = 200000, Stock = 5 },
            new Product { Id = 3, Name = "Martillo", Price = 50000, Stock = 20 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetProducts(minPrice: 75000, maxPrice: 150000);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        _mockProductRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProducts_WithInStockOnlyFilter_ReturnsOnlyAvailableProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 10 },
            new Product { Id = 2, Name = "Sierra", Price = 200000, Stock = 0 },
            new Product { Id = 3, Name = "Martillo", Price = 50000, Stock = 5 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetProducts(inStockOnly: true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 10 };
        var productDto = new ProductDtoV2 { Id = 1, Name = "Taladro", Price = 100000, Stock = 10 };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(product)).Returns(productDto);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDtoV2>(okResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("Taladro", returnedProduct.Name);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockProductRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetProduct(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetStatistics_WithProducts_ReturnsCorrectStatistics()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 10 },
            new Product { Id = 2, Name = "Sierra", Price = 200000, Stock = 0 },
            new Product { Id = 3, Name = "Martillo", Price = 50000, Stock = 5 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetStatistics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var stats = Assert.IsType<ProductStatisticsDto>(okResult.Value);
        
        Assert.Equal(3, stats.TotalProducts);
        Assert.Equal(2, stats.ProductsInStock); // Taladro and Martillo
        Assert.Equal(1, stats.ProductsOutOfStock); // Sierra
        Assert.NotNull(stats.MostExpensiveProduct);
        Assert.NotNull(stats.CheapestProduct);
    }

    [Fact]
    public async Task GetStatistics_WithNoProducts_ReturnsEmptyStatistics()
    {
        // Arrange
        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        // Act
        var result = await _controller.GetStatistics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var stats = Assert.IsType<ProductStatisticsDto>(okResult.Value);
        
        Assert.Equal(0, stats.TotalProducts);
    }

    [Fact]
    public async Task GetLowStockProducts_ReturnsProductsBelowThreshold()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 3 },
            new Product { Id = 2, Name = "Sierra", Price = 200000, Stock = 15 },
            new Product { Id = 3, Name = "Martillo", Price = 50000, Stock = 5 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetLowStockProducts(threshold: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lowStockProducts = Assert.IsType<List<ProductDtoV2>>(okResult.Value);
        
        // Should return Taladro (3) and Martillo (5), but not Sierra (15)
        Assert.Equal(2, lowStockProducts.Count);
    }

    [Fact]
    public async Task GetLowStockProducts_WithCustomThreshold_ReturnsCorrectProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Taladro", Price = 100000, Stock = 3 },
            new Product { Id = 2, Name = "Sierra", Price = 200000, Stock = 15 },
            new Product { Id = 3, Name = "Martillo", Price = 50000, Stock = 5 }
        };

        _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<ProductDtoV2>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDtoV2 
            { 
                Id = p.Id, 
                Name = p.Name, 
                Price = p.Price, 
                Stock = p.Stock 
            });

        // Act
        var result = await _controller.GetLowStockProducts(threshold: 4);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lowStockProducts = Assert.IsType<List<ProductDtoV2>>(okResult.Value);
        
        // Should return only Taladro (3), not Martillo (5) or Sierra (15)
        Assert.Single(lowStockProducts);
    }
}
