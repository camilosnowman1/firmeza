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
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomersController(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    // GET: api/Customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await _customerRepository.GetAllAsync();
        var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        return Ok(customerDtos);
    }

    // GET: api/Customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        var customerDto = _mapper.Map<CustomerDto>(customer);
        return Ok(customerDto);
    }

    // POST: api/Customers
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> PostCustomer(CreateCustomerDto createCustomerDto)
    {
        var customer = _mapper.Map<Customer>(createCustomerDto);
        
        // Optional: Check if document already exists to return a more specific error
        // var existingCustomer = (await _customerRepository.GetAllAsync()).FirstOrDefault(c => c.Document == customer.Document);
        // if (existingCustomer != null) return BadRequest("A customer with this document already exists.");

        await _customerRepository.AddAsync(customer);

        var customerDto = _mapper.Map<CustomerDto>(customer);

        return CreatedAtAction(nameof(GetCustomer), new { id = customerDto.Id }, customerDto);
    }

    // PUT: api/Customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, UpdateCustomerDto updateCustomerDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        _mapper.Map(updateCustomerDto, customer);
        await _customerRepository.UpdateAsync(customer);

        return NoContent();
    }

    // DELETE: api/Customers/5
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