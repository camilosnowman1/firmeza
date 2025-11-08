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
public class RentalsController : ControllerBase
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public RentalsController(IRentalRepository rentalRepository, ICustomerRepository customerRepository, IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    // GET: api/v1/Rentals
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RentalDto>>> GetRentals()
    {
        var rentals = await _rentalRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<RentalDto>>(rentals));
    }

    // GET: api/v1/Rentals/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDto>> GetRental(int id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<RentalDto>(rental));
    }

    // POST: api/v1/Rentals
    [HttpPost]
    public async Task<ActionResult<RentalDto>> PostRental(CreateRentalDto createRentalDto)
    {
        var customer = await _customerRepository.GetByIdAsync(createRentalDto.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found.");
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(createRentalDto.VehicleId);
        if (vehicle == null)
        {
            return BadRequest("Vehicle not found.");
        }

        if (createRentalDto.EndDate <= createRentalDto.StartDate)
        {
            return BadRequest("End date must be after start date.");
        }

        var rental = _mapper.Map<Rental>(createRentalDto);
        rental.TotalAmount = (decimal)(rental.EndDate - rental.StartDate).TotalHours * vehicle.HourlyRate;
        rental.CreatedAt = DateTime.UtcNow;

        await _rentalRepository.AddAsync(rental);

        return CreatedAtAction(nameof(GetRental), new { id = rental.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, _mapper.Map<RentalDto>(rental));
    }

    // PUT: api/v1/Rentals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRental(int id, UpdateRentalDto updateRentalDto)
    {
        var existingRental = await _rentalRepository.GetByIdAsync(id);
        if (existingRental == null)
        {
            return NotFound();
        }
        
        var customer = await _customerRepository.GetByIdAsync(updateRentalDto.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found.");
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(updateRentalDto.VehicleId);
        if (vehicle == null)
        {
            return BadRequest("Vehicle not found.");
        }

        if (updateRentalDto.EndDate <= updateRentalDto.StartDate)
        {
            return BadRequest("End date must be after start date.");
        }

        _mapper.Map(updateRentalDto, existingRental);
        existingRental.TotalAmount = (decimal)(existingRental.EndDate - existingRental.StartDate).TotalHours * vehicle.HourlyRate;

        await _rentalRepository.UpdateAsync(existingRental);

        return NoContent();
    }

    // DELETE: api/v1/Rentals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRental(int id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            return NotFound();
        }

        await _rentalRepository.DeleteAsync(id);
        return NoContent();
    }
}
