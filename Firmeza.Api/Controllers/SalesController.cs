using AutoMapper;
using Firmeza.Api.Dtos;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // Protect all endpoints in this controller by default
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public SalesController(ISaleRepository saleRepository, IProductRepository productRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // GET: api/Sales
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
    {
        var sales = await _saleRepository.GetAllAsync();
        var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
        return Ok(saleDtos);
    }

    // GET: api/Sales/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetSale(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);

        if (sale == null)
        {
            return NotFound();
        }

        var saleDto = _mapper.Map<SaleDto>(sale);
        return Ok(saleDto);
    }

    // POST: api/Sales
    [HttpPost]
    public async Task<ActionResult<SaleDto>> PostSale(CreateSaleDto createSaleDto)
    {
        var sale = _mapper.Map<Sale>(createSaleDto);
        sale.SaleDate = DateTime.UtcNow;

        // Manually calculate TotalAmount and ensure UnitPrice from current product price
        foreach (var detail in sale.SaleDetails)
        {
            var product = await _productRepository.GetByIdAsync(detail.ProductId);
            if (product == null)
            {
                return BadRequest($"Product with ID {detail.ProductId} not found.");
            }
            detail.UnitPrice = product.Price;
            sale.TotalAmount += detail.TotalPrice;
        }

        await _saleRepository.AddAsync(sale);

        var saleDto = _mapper.Map<SaleDto>(sale);

        return CreatedAtAction(nameof(GetSale), new { id = saleDto.Id }, saleDto);
    }

    // PUT: api/Sales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSale(int id, UpdateSaleDto updateSaleDto)
    {
        var existingSale = await _saleRepository.GetByIdAsync(id);
        if (existingSale == null)
        {
            return NotFound();
        }

        // Update basic sale properties
        _mapper.Map(updateSaleDto, existingSale);
        existingSale.SaleDate = DateTime.UtcNow; // Update sale date on modification

        // Handle SaleDetails: remove old, add new, update existing
        existingSale.SaleDetails.Clear(); // Clear existing details
        existingSale.TotalAmount = 0;

        foreach (var newDetailDto in updateSaleDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(newDetailDto.ProductId);
            if (product == null)
            {
                return BadRequest($"Product with ID {newDetailDto.ProductId} not found.");
            }

            var newDetail = _mapper.Map<SaleDetail>(newDetailDto);
            newDetail.UnitPrice = product.Price;
            existingSale.SaleDetails.Add(newDetail);
            existingSale.TotalAmount += newDetail.TotalPrice;
        }

        await _saleRepository.UpdateAsync(existingSale);

        return NoContent();
    }

    // DELETE: api/Sales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }

        await _saleRepository.DeleteAsync(id);

        return NoContent();
    }
}