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
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public CustomersController(ICustomerRepository customerRepository, IEmailService emailService, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _emailService = emailService;
        _mapper = mapper;
    }

    // GET: api/v1/Customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
    }

    // GET: api/v1/Customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    // POST: api/v1/Customers
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> PostCustomer(CreateCustomerDto createCustomerDto)
    {
        var customer = _mapper.Map<Customer>(createCustomerDto);
        await _customerRepository.AddAsync(customer);
        
        // Send welcome email
        try
        {
            await _emailService.SendEmailAsync(customer.Email, "Welcome to Firmeza API!", "<h1>Thank you for registering!</h1><p>Your API account has been created successfully.</p>");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send welcome email from API: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, _mapper.Map<CustomerDto>(customer));
    }

    // PUT: api/v1/Customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, UpdateCustomerDto updateCustomerDto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        _mapper.Map(updateCustomerDto, existingCustomer);
        await _customerRepository.UpdateAsync(existingCustomer);

        return NoContent();
    }

    // DELETE: api/v1/Customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        await _customerRepository.DeleteAsync(id);
        return NoContent();
    }
}
