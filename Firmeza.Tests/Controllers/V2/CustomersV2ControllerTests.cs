using Xunit;
using Moq;
using AutoMapper;
using Firmeza.Api.Controllers.V2;
using Firmeza.Api.Dtos.V2;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Tests.Controllers.V2;

public class CustomersV2ControllerTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<ISaleRepository> _mockSaleRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CustomersV2Controller _controller;

    public CustomersV2ControllerTests()
    {
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockSaleRepo = new Mock<ISaleRepository>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CustomersV2Controller(_mockCustomerRepo.Object, _mockSaleRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetCustomers_WithoutSearch_ReturnsAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FullName = "Juan Pérez", Email = "juan@test.com", Document = "123456" },
            new Customer { Id = 2, FullName = "María García", Email = "maria@test.com", Document = "789012" }
        };

        _mockCustomerRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<CustomerDtoV2>(It.IsAny<Customer>()))
            .Returns((Customer c) => new CustomerDtoV2 
            { 
                Id = c.Id, 
                FullName = c.FullName, 
                Email = c.Email,
                Document = c.Document
            });

        // Act
        var result = await _controller.GetCustomers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        _mockCustomerRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCustomers_WithSearch_ReturnsFilteredCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FullName = "Juan Pérez", Email = "juan@test.com", Document = "123456" },
            new Customer { Id = 2, FullName = "María García", Email = "maria@test.com", Document = "789012" },
            new Customer { Id = 3, FullName = "Pedro López", Email = "pedro@test.com", Document = "345678" }
        };

        _mockCustomerRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Sale>());
        
        _mockMapper.Setup(m => m.Map<CustomerDtoV2>(It.IsAny<Customer>()))
            .Returns((Customer c) => new CustomerDtoV2 
            { 
                Id = c.Id, 
                FullName = c.FullName, 
                Email = c.Email,
                Document = c.Document
            });

        // Act
        var result = await _controller.GetCustomers(search: "juan");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetCustomer_WithValidId_ReturnsCustomerWithPurchaseInfo()
    {
        // Arrange
        var customer = new Customer 
        { 
            Id = 1, 
            FullName = "Juan Pérez", 
            Email = "juan@test.com", 
            Document = "123456" 
        };

        var sales = new List<Sale>
        {
            new Sale { Id = 1, CustomerId = 1, TotalAmount = 100000, SaleDate = DateTime.Now },
            new Sale { Id = 2, CustomerId = 1, TotalAmount = 50000, SaleDate = DateTime.Now.AddDays(-5) }
        };

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);
        
        _mockMapper.Setup(m => m.Map<CustomerDtoV2>(customer))
            .Returns(new CustomerDtoV2 
            { 
                Id = customer.Id, 
                FullName = customer.FullName, 
                Email = customer.Email,
                Document = customer.Document
            });

        // Act
        var result = await _controller.GetCustomer(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var customerDto = Assert.IsType<CustomerDtoV2>(okResult.Value);
        
        Assert.Equal(1, customerDto.Id);
        Assert.Equal("Juan Pérez", customerDto.FullName);
    }

    [Fact]
    public async Task GetCustomer_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer)null);

        // Act
        var result = await _controller.GetCustomer(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPurchaseHistory_WithValidCustomerId_ReturnsHistory()
    {
        // Arrange
        var customer = new Customer { Id = 1, FullName = "Juan Pérez" };
        
        var product1 = new Product { Id = 1, Name = "Taladro", Price = 100000 };
        var product2 = new Product { Id = 2, Name = "Sierra", Price = 50000 };

        var sales = new List<Sale>
        {
            new Sale 
            { 
                Id = 1, 
                CustomerId = 1, 
                TotalAmount = 150000, 
                SaleDate = DateTime.Now,
                SaleDetails = new List<SaleDetail>
                {
                    new SaleDetail { ProductId = 1, Product = product1, Quantity = 1, UnitPrice = 100000, TotalPrice = 100000 },
                    new SaleDetail { ProductId = 2, Product = product2, Quantity = 1, UnitPrice = 50000, TotalPrice = 50000 }
                }
            }
        };

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);

        // Act
        var result = await _controller.GetPurchaseHistory(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var history = Assert.IsType<List<PurchaseHistoryDto>>(okResult.Value);
        
        Assert.Single(history);
        Assert.Equal(2, history[0].ItemCount);
        Assert.Equal(150000, history[0].TotalAmount);
    }

    [Fact]
    public async Task GetPurchaseHistory_WithInvalidCustomerId_ReturnsNotFound()
    {
        // Arrange
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer)null);

        // Act
        var result = await _controller.GetPurchaseHistory(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetFrequentCustomers_ReturnsCustomersWithMinimumPurchases()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FullName = "Juan Pérez", Email = "juan@test.com", Document = "123" },
            new Customer { Id = 2, FullName = "María García", Email = "maria@test.com", Document = "456" },
            new Customer { Id = 3, FullName = "Pedro López", Email = "pedro@test.com", Document = "789" }
        };

        var sales = new List<Sale>
        {
            // Juan: 6 compras (frecuente)
            new Sale { Id = 1, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 2, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 3, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 4, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 5, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 6, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            
            // María: 3 compras (no frecuente)
            new Sale { Id = 7, CustomerId = 2, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 8, CustomerId = 2, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 9, CustomerId = 2, TotalAmount = 10000, SaleDate = DateTime.Now },
            
            // Pedro: 5 compras (frecuente)
            new Sale { Id = 10, CustomerId = 3, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 11, CustomerId = 3, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 12, CustomerId = 3, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 13, CustomerId = 3, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 14, CustomerId = 3, TotalAmount = 10000, SaleDate = DateTime.Now }
        };

        _mockCustomerRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);
        
        _mockMapper.Setup(m => m.Map<CustomerDtoV2>(It.IsAny<Customer>()))
            .Returns((Customer c) => new CustomerDtoV2 
            { 
                Id = c.Id, 
                FullName = c.FullName, 
                Email = c.Email,
                Document = c.Document
            });

        // Act
        var result = await _controller.GetFrequentCustomers(minPurchases: 5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var frequentCustomers = Assert.IsType<List<CustomerDtoV2>>(okResult.Value);
        
        // Should return Juan (6) and Pedro (5), but not María (3)
        Assert.Equal(2, frequentCustomers.Count);
    }

    [Fact]
    public async Task GetFrequentCustomers_WithCustomThreshold_ReturnsCorrectCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FullName = "Juan Pérez", Email = "juan@test.com", Document = "123" },
            new Customer { Id = 2, FullName = "María García", Email = "maria@test.com", Document = "456" }
        };

        var sales = new List<Sale>
        {
            new Sale { Id = 1, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 2, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 3, CustomerId = 1, TotalAmount = 10000, SaleDate = DateTime.Now },
            new Sale { Id = 4, CustomerId = 2, TotalAmount = 10000, SaleDate = DateTime.Now }
        };

        _mockCustomerRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);
        _mockSaleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);
        
        _mockMapper.Setup(m => m.Map<CustomerDtoV2>(It.IsAny<Customer>()))
            .Returns((Customer c) => new CustomerDtoV2 
            { 
                Id = c.Id, 
                FullName = c.FullName, 
                Email = c.Email,
                Document = c.Document
            });

        // Act
        var result = await _controller.GetFrequentCustomers(minPurchases: 3);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var frequentCustomers = Assert.IsType<List<CustomerDtoV2>>(okResult.Value);
        
        // Should return only Juan (3 purchases), not María (1 purchase)
        Assert.Single(frequentCustomers);
    }
}
