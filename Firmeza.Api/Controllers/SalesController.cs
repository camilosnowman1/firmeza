using AutoMapper;
using Firmeza.Api.Dtos;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public SalesController(ISaleRepository saleRepository, ICustomerRepository customerRepository, IProductRepository productRepository, IMapper mapper, IEmailService emailService)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _emailService = emailService;
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
        sale.TotalAmount = 0;

        var emailBody = new StringBuilder();
        emailBody.AppendLine("<thead><tr><th>Producto</th><th>Cantidad</th><th>Precio Unitario</th><th>Total</th></tr></thead><tbody>");

        foreach (var detailDto in createSaleDto.Items)
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
            saleDetail.TotalPrice = product.Price * detailDto.Quantity;
            sale.TotalAmount += saleDetail.TotalPrice;
            sale.SaleDetails.Add(saleDetail);

            product.Stock -= detailDto.Quantity;
            await _productRepository.UpdateAsync(product);
            
            emailBody.AppendLine($"<tr><td>{product.Name}</td><td>{detailDto.Quantity}</td><td>{saleDetail.UnitPrice:C}</td><td>{saleDetail.TotalPrice:C}</td></tr>");
        }
        
        emailBody.AppendLine("</tbody>");

        await _saleRepository.AddAsync(sale);

        // Send confirmation email
        try
        {
            var subject = $"Confirmación de tu compra #{sale.Id}";
            var fullEmailBody = $@"
                <h1>¡Gracias por tu compra, {customer.FullName}!</h1>
                <p>Hemos recibido tu pedido. Aquí están los detalles:</p>
                <table border='1' cellpadding='10' style='border-collapse: collapse; width: 100%;'>
                    {emailBody}
                </table>
                <h3 style='text-align: right; margin-top: 20px;'>Total: {sale.TotalAmount:C}</h3>
                <hr>
                <p>Equipo de Firmeza</p>";
            
            await _emailService.SendEmailAsync(customer.Email, subject, fullEmailBody);
        }
        catch (Exception ex)
        {
            // Log the exception but don't fail the request if the email fails
            Console.WriteLine($"Failed to send confirmation email for sale {sale.Id}: {ex.Message}");
        }

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
