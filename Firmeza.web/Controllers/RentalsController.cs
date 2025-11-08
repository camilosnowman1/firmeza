using Firmeza.Core.Entities;
using Firmeza.Infrastructure.Persistence;
using Firmeza.Web.Documents;
using Firmeza.Web.Helpers;
using Firmeza.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class RentalsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RentalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var rentals = await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
            
        return View(rentals);
    }

    public async Task<IActionResult> Create()
    {
        var customers = await _context.Customers.ToListAsync();
        var vehicles = await _context.Vehicles.ToListAsync();

        var model = new RentalViewModel
        {
            Customers = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName }).ToList(),
            Vehicles = vehicles.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = $"{v.Name} (${v.HourlyRate}/hr)" }).ToList(),
            VehicleRates = vehicles.ToDictionary(v => v.Id, v => v.HourlyRate)
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RentalViewModel model)
    {
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("EndDate", "End date must be after start date.");
        }

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is INVALID. Errors:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"- {error.ErrorMessage}");
            }
            
            // Repopulate dropdowns if model is not valid
            var customers = await _context.Customers.ToListAsync();
            var vehicles = await _context.Vehicles.ToListAsync();
            model.Customers = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName }).ToList();
            model.Vehicles = vehicles.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = $"{v.Name} (${v.HourlyRate}/hr)" }).ToList();
            model.VehicleRates = vehicles.ToDictionary(v => v.Id, v => v.HourlyRate);
            return View(model);
        }
        
        Console.WriteLine("ModelState is VALID. Proceeding to save.");

        try
        {
            var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
            if (vehicle == null)
            {
                ModelState.AddModelError("VehicleId", "Selected vehicle not found.");
                return View(model);
            }

            var rental = new Rental
            {
                CustomerId = model.CustomerId,
                VehicleId = model.VehicleId,
                StartDate = model.StartDate.ToUniversalTime(),
                EndDate = model.EndDate.ToUniversalTime(),
                TotalAmount = (decimal)(model.EndDate - model.StartDate).TotalHours * vehicle.HourlyRate
            };
            
            _context.Add(rental);
            await _context.SaveChangesAsync();
            Console.WriteLine("SaveChanges was successful!");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occurred while saving the rental: {ex.ToString()}");
            ModelState.AddModelError("", "An unexpected error occurred while saving the rental. Please try again.");
            
            // Repopulate dropdowns on error
            var customers = await _context.Customers.ToListAsync();
            var vehicles = await _context.Vehicles.ToListAsync();
            model.Customers = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName }).ToList();
            model.Vehicles = vehicles.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = $"{v.Name} (${v.HourlyRate}/hr)" }).ToList();
            model.VehicleRates = vehicles.ToDictionary(v => v.Id, v => v.HourlyRate);
            return View(model);
        }
    }
    
    // New Details Method for Rentals
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var rental = await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (rental == null)
        {
            return NotFound();
        }

        return View(rental);
    }
    
    // New GenerateContract Method for Rentals
    public async Task<IActionResult> GenerateContract(int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id);
        
        if (rental == null)
        {
            return NotFound();
        }

        var document = new RentalContractDocument(rental);
        var pdf = document.GeneratePdf();
        return File(pdf, "application/pdf", $"RentalContract-{id}.pdf");
    }
}
