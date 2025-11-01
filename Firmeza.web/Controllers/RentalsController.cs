using Firmeza.Web.Data;
using Firmeza.Web.Data.Entities;
using Firmeza.Web.Helpers;
using Firmeza.Web.Documents; // Import the PDF document class
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent; // Import QuestPDF
using System;
using System.Collections.Generic;
using System.IO; // Import System.IO for file operations
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Web.Controllers;

public class RentalViewModel
{
    public int CustomerId { get; set; }
    public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    public int VehicleId { get; set; }
    public List<SelectListItem> Vehicles { get; set; } = new List<SelectListItem>();
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);
}

[Authorize(Roles = "Admin")]
public class RentalsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public RentalsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Rentals
    public async Task<IActionResult> Index(int? pageNumber)
    {
        int pageSize = 10;
        var rentals = _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .AsNoTracking();

        return View(await PaginatedList<Rental>.CreateAsync(rentals.OrderByDescending(r => r.StartDate), pageNumber ?? 1, pageSize));
    }

    // GET: Rentals/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = new RentalViewModel
        {
            Customers = await _context.Customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            }).ToListAsync(),
            Vehicles = await _context.Vehicles.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Name + " (Hourly: " + v.HourlyRate.ToString("C") + ")"
            }).ToListAsync()
        };
        return View(viewModel);
    }

    // POST: Rentals/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RentalViewModel model)
    {
        if (model.CustomerId == 0 || model.VehicleId == 0 || model.StartDate >= model.EndDate)
        {
            TempData["ErrorMessage"] = "Please select a customer, a vehicle, and ensure start date is before end date.";
            return RedirectToAction(nameof(Create));
        }

        var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
        if (vehicle == null)
        {
            TempData["ErrorMessage"] = "Selected vehicle not found.";
            return RedirectToAction(nameof(Create));
        }

        var rental = new Rental
        {
            CustomerId = model.CustomerId,
            VehicleId = model.VehicleId,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            TotalAmount = (decimal)model.TotalHours * vehicle.HourlyRate
        };

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        // --- PDF Generation ---
        try
        {
            var rentalWithDetails = await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == rental.Id);

            if (rentalWithDetails != null)
            {
                var document = new RentalContractDocument(rentalWithDetails);
                var pdfBytes = document.GeneratePdf();

                var contractsDir = Path.Combine(_webHostEnvironment.WebRootPath, "contracts");
                if (!Directory.Exists(contractsDir)) Directory.CreateDirectory(contractsDir);

                var filePath = Path.Combine(contractsDir, $"contract_{rental.Id}.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
            }
        }
        catch (Exception ex)
        {
            TempData["PdfGenerationError"] = $"Rental was created successfully, but failed to generate contract PDF. Error: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Rentals/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null)
        {
            return NotFound();
        }

        var viewModel = new RentalViewModel
        {
            CustomerId = rental.CustomerId,
            VehicleId = rental.VehicleId,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            Customers = await _context.Customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            }).ToListAsync(),
            Vehicles = await _context.Vehicles.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Name + " (Hourly: " + v.HourlyRate.ToString("C") + ")"
            }).ToListAsync()
        };
        return View(viewModel);
    }

    // POST: Rentals/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RentalViewModel model)
    {
        if (id == 0)
        {
            return NotFound();
        }

        if (model.CustomerId == 0 || model.VehicleId == 0 || model.StartDate >= model.EndDate)
        {
            TempData["ErrorMessage"] = "Please select a customer, a vehicle, and ensure start date is before end date.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
        if (vehicle == null)
        {
            TempData["ErrorMessage"] = "Selected vehicle not found.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        rental.CustomerId = model.CustomerId;
        rental.VehicleId = model.VehicleId;
        rental.StartDate = model.StartDate;
        rental.EndDate = model.EndDate;
        rental.TotalAmount = (decimal)rental.TotalHours * vehicle.HourlyRate;

        _context.Update(rental);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Rentals/Delete/5
    public async Task<IActionResult> Delete(int? id)
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

    // POST: Rentals/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}