using AutoMapper;
using Firmeza.Api.Dtos;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")] // Added API Version
[Route("api/v{version:apiVersion}/[controller]")] // Updated Route
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public SalesController(ISaleRepository saleRepository, ICustomerRepository customerRepository, IProductRepository productRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // GET: api/v1/Sales
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
    {
        var sales = await _saleRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<SaleDto>>(sales));
    }

    // GET: api/v1/Sales/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetSale(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<SaleDto>(sale));
    }

    // POST: api/v1/Sales
    [HttpPost]
    public async Task<ActionResult<SaleDto>> PostSale(CreateSaleDto createSaleDto)
    {
        var customer = await _customerRepository.GetByIdAsync(createSaleDto.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found.");
        }

        var sale = _mapper.Map<Sale>(createSaleDto);
        sale.SaleDate = DateTime.UtcNow;
        sale.TotalAmount = 0; // Will be calculated based on details

        foreach (var detailDto in createSaleDto.Items) // Changed from createSaleDto.SaleDetails to createSaleDto.Items
        {
            var product = await _productRepository.GetByIdAsync(detailDto.ProductId);
            if (product == null)
            {
                return BadRequest($"Product with ID {detailDto.ProductId} not found.");
            }
            if (product.Stock < detailDto.Quantity)
            {
                return BadRequest($"Not enough stock for product {product.Name}. Available: {product.Stock}, Requested: {detailDto.Quantity}");
            }

            var saleDetail = _mapper.Map<SaleDetail>(detailDto);
            saleDetail.UnitPrice = product.Price;
            saleDetail.TotalPrice = product.Price * detailDto.Quantity; // This line will be fixed next
            sale.TotalAmount += saleDetail.TotalPrice;
            sale.SaleDetails.Add(saleDetail);

            product.Stock -= detailDto.Quantity; // Reduce stock
            await _productRepository.UpdateAsync(product);
        }

        await _saleRepository.AddAsync(sale);

        return CreatedAtAction(nameof(GetSale), new { id = sale.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, _mapper.Map<SaleDto>(sale));
    }

    // PUT: api/v1/Sales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSale(int id, UpdateSaleDto updateSaleDto)
    {
        var existingSale = await _saleRepository.GetByIdAsync(id);
        if (existingSale == null)
        {
            return NotFound();
        }

        var customer = await _customerRepository.GetByIdAsync(updateSaleDto.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found.");
        }

        _mapper.Map(updateSaleDto, existingSale);
        // Recalculate total amount and handle stock changes if sale details are updated
        // This is a complex operation and might require more sophisticated logic for a real-world scenario
        // For simplicity, we are just updating the main sale properties here.
        await _saleRepository.UpdateAsync(existingSale);

        return NoContent();
    }

    // DELETE: api/v1/Sales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }

        // Restore stock for products in this sale
        foreach (var detail in sale.SaleDetails)
        {
            var product = await _productRepository.GetByIdAsync(detail.ProductId);
            if (product != null)
            {
                product.Stock += detail.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        await _saleRepository.DeleteAsync(id);
        return NoContent();
    }
}
