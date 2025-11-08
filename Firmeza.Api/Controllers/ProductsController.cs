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
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // GET: api/v1/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    // GET: api/v1/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<ProductDto>(product));
    }

    // POST: api/v1/Products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        await _productRepository.AddAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, _mapper.Map<ProductDto>(product));
    }

    // PUT: api/v1/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, UpdateProductDto updateProductDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        _mapper.Map(updateProductDto, existingProduct);
        await _productRepository.UpdateAsync(existingProduct);

        return NoContent();
    }

    // DELETE: api/v1/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productRepository.DeleteAsync(id);
        return NoContent();
    }
}
