using AutoMapper;
using Firmeza.Api.Dtos.V2;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Api.Controllers.V2;

[Authorize]
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsV2Controller : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ProductsV2Controller(IProductRepository productRepository, ISaleRepository saleRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all products with advanced filtering and search (v2)
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 9)</param>
    /// <param name="search">Search by name or description</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="inStockOnly">Show only products in stock</param>
    /// <param name="sortBy">Sort by: name, price, stock (default: name)</param>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 9,
        [FromQuery] string? search = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool inStockOnly = false,
        [FromQuery] string sortBy = "name")
    {
        var allProducts = await _productRepository.GetAllAsync();
        var allSales = await _saleRepository.GetAllAsync();

        // Apply filters
        var filteredProducts = allProducts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            filteredProducts = filteredProducts.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (minPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice.Value);
        }

        if (inStockOnly)
        {
            filteredProducts = filteredProducts.Where(p => p.Stock > 0);
        }

        // Apply sorting
        filteredProducts = sortBy.ToLower() switch
        {
            "price" => filteredProducts.OrderBy(p => p.Price),
            "stock" => filteredProducts.OrderByDescending(p => p.Stock),
            _ => filteredProducts.OrderBy(p => p.Name)
        };

        var totalCount = filteredProducts.Count();

        // Apply pagination
        var products = filteredProducts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Map to V2 DTOs with sales count
        var productDtos = products.Select(p =>
        {
            var dto = _mapper.Map<ProductDtoV2>(p);
            dto.TotalSales = allSales
                .SelectMany(s => s.SaleDetails)
                .Where(sd => sd.ProductId == p.Id)
                .Sum(sd => sd.Quantity);
            return dto;
        }).ToList();

        var response = new
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = productDtos,
            // V2: Additional metadata
            Filters = new
            {
                Search = search,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                InStockOnly = inStockOnly,
                SortBy = sortBy
            }
        };

        // V2: Add pagination info to headers
        Response.Headers.Add("X-Total-Count", totalCount.ToString());
        Response.Headers.Add("X-Page-Number", pageNumber.ToString());
        Response.Headers.Add("X-Page-Size", pageSize.ToString());

        return Ok(response);
    }

    /// <summary>
    /// Get product by ID with sales information (v2)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDtoV2>> GetProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        var allSales = await _saleRepository.GetAllAsync();
        var dto = _mapper.Map<ProductDtoV2>(product);
        dto.TotalSales = allSales
            .SelectMany(s => s.SaleDetails)
            .Where(sd => sd.ProductId == id)
            .Sum(sd => sd.Quantity);

        return Ok(dto);
    }

    /// <summary>
    /// Get product statistics (v2 only)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("statistics")]
    public async Task<ActionResult<ProductStatisticsDto>> GetStatistics()
    {
        var products = (await _productRepository.GetAllAsync()).ToList();

        if (!products.Any())
        {
            return Ok(new ProductStatisticsDto());
        }

        var allSales = await _saleRepository.GetAllAsync();

        var stats = new ProductStatisticsDto
        {
            TotalProducts = products.Count,
            AveragePrice = products.Average(p => p.Price),
            TotalInventoryValue = products.Sum(p => p.Price * p.Stock),
            ProductsInStock = products.Count(p => p.Stock > 0),
            ProductsOutOfStock = products.Count(p => p.Stock == 0),
            LowStockProducts = products.Count(p => p.Stock > 0 && p.Stock < 10)
        };

        var mostExpensive = products.OrderByDescending(p => p.Price).FirstOrDefault();
        var cheapest = products.OrderBy(p => p.Price).FirstOrDefault();

        if (mostExpensive != null)
        {
            stats.MostExpensiveProduct = _mapper.Map<ProductDtoV2>(mostExpensive);
            stats.MostExpensiveProduct.TotalSales = allSales
                .SelectMany(s => s.SaleDetails)
                .Where(sd => sd.ProductId == mostExpensive.Id)
                .Sum(sd => sd.Quantity);
        }

        if (cheapest != null)
        {
            stats.CheapestProduct = _mapper.Map<ProductDtoV2>(cheapest);
            stats.CheapestProduct.TotalSales = allSales
                .SelectMany(s => s.SaleDetails)
                .Where(sd => sd.ProductId == cheapest.Id)
                .Sum(sd => sd.Quantity);
        }

        return Ok(stats);
    }

    /// <summary>
    /// Get products with low stock (v2 only)
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDtoV2>>> GetLowStockProducts([FromQuery] int threshold = 10)
    {
        var products = (await _productRepository.GetAllAsync())
            .Where(p => p.Stock > 0 && p.Stock < threshold)
            .OrderBy(p => p.Stock)
            .ToList();

        var allSales = await _saleRepository.GetAllAsync();

        var productDtos = products.Select(p =>
        {
            var dto = _mapper.Map<ProductDtoV2>(p);
            dto.TotalSales = allSales
                .SelectMany(s => s.SaleDetails)
                .Where(sd => sd.ProductId == p.Id)
                .Sum(sd => sd.Quantity);
            return dto;
        }).ToList();

        return Ok(productDtos);
    }
}
