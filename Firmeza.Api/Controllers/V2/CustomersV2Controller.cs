using AutoMapper;
using Firmeza.Api.Dtos.V2;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers.V2;

[Authorize]
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CustomersV2Controller : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CustomersV2Controller(ICustomerRepository customerRepository, ISaleRepository saleRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all customers with advanced search (v2)
    /// </summary>
    /// <param name="search">Search by name, email, or document</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    [HttpGet]
    public async Task<ActionResult> GetCustomers(
        [FromQuery] string? search = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var allCustomers = await _customerRepository.GetAllAsync();
        var allSales = await _saleRepository.GetAllAsync();

        // Apply search filter
        var filteredCustomers = allCustomers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            filteredCustomers = filteredCustomers.Where(c =>
                c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Document.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = filteredCustomers.Count();

        // Apply pagination
        var customers = filteredCustomers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Map to V2 DTOs with purchase information
        var customerDtos = customers.Select(c =>
        {
            var dto = _mapper.Map<CustomerDtoV2>(c);
            var customerSales = allSales.Where(s => s.CustomerId == c.Id).ToList();
            dto.TotalPurchases = customerSales.Count;
            dto.TotalSpent = customerSales.Sum(s => s.TotalAmount);
            dto.LastPurchaseDate = customerSales.OrderByDescending(s => s.SaleDate).FirstOrDefault()?.SaleDate;
            return dto;
        }).ToList();

        var response = new
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = customerDtos
        };

        return Ok(response);
    }

    /// <summary>
    /// Get customer by ID with purchase summary (v2)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDtoV2>> GetCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound(new { message = $"Customer with ID {id} not found" });
        }

        var allSales = await _saleRepository.GetAllAsync();
        var dto = _mapper.Map<CustomerDtoV2>(customer);
        var customerSales = allSales.Where(s => s.CustomerId == id).ToList();
        
        dto.TotalPurchases = customerSales.Count;
        dto.TotalSpent = customerSales.Sum(s => s.TotalAmount);
        dto.LastPurchaseDate = customerSales.OrderByDescending(s => s.SaleDate).FirstOrDefault()?.SaleDate;

        return Ok(dto);
    }

    /// <summary>
    /// Get customer purchase history (v2 only)
    /// </summary>
    [HttpGet("{id}/purchase-history")]
    public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetPurchaseHistory(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound(new { message = $"Customer with ID {id} not found" });
        }

        var allSales = await _saleRepository.GetAllAsync();
        var customerSales = allSales
            .Where(s => s.CustomerId == id)
            .OrderByDescending(s => s.SaleDate)
            .ToList();

        var history = customerSales.Select(sale => new PurchaseHistoryDto
        {
            SaleId = sale.Id,
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            ItemCount = sale.SaleDetails.Count,
            Items = sale.SaleDetails.Select(sd => new PurchaseItemDto
            {
                ProductName = sd.Product?.Name ?? "Unknown Product",
                Quantity = sd.Quantity,
                UnitPrice = sd.UnitPrice,
                TotalPrice = sd.TotalPrice
            }).ToList()
        }).ToList();

        return Ok(history);
    }

    /// <summary>
    /// Get frequent customers (5+ purchases) (v2 only)
    /// </summary>
    [HttpGet("frequent")]
    public async Task<ActionResult<IEnumerable<CustomerDtoV2>>> GetFrequentCustomers([FromQuery] int minPurchases = 5)
    {
        var allCustomers = await _customerRepository.GetAllAsync();
        var allSales = await _saleRepository.GetAllAsync();

        var frequentCustomers = allCustomers
            .Select(c =>
            {
                var dto = _mapper.Map<CustomerDtoV2>(c);
                var customerSales = allSales.Where(s => s.CustomerId == c.Id).ToList();
                dto.TotalPurchases = customerSales.Count;
                dto.TotalSpent = customerSales.Sum(s => s.TotalAmount);
                dto.LastPurchaseDate = customerSales.OrderByDescending(s => s.SaleDate).FirstOrDefault()?.SaleDate;
                return dto;
            })
            .Where(dto => dto.TotalPurchases >= minPurchases)
            .OrderByDescending(dto => dto.TotalPurchases)
            .ToList();

        return Ok(frequentCustomers);
    }
}
