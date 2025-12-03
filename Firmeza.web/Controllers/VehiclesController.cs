using Firmeza.Core.Entities;
using Firmeza.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class VehiclesController : Controller
{
    private readonly Infrastructure.Persistence.ApplicationDbContext _context;

    public VehiclesController(Infrastructure.Persistence.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        ViewData["CurrentFilter"] = searchString;
        var vehicles = from v in _context.Vehicles select v;

        if (!string.IsNullOrEmpty(searchString))
        {
            vehicles = vehicles.Where(s => s.Name.Contains(searchString));
        }

        int pageSize = 10;
        return View(await PaginatedList<Vehicle>.CreateAsync(vehicles.OrderBy(v => v.Name), pageNumber ?? 1, pageSize));
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(vehicle);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null) return NotFound();
        return View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(vehicle);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null) return NotFound();
        return View(vehicle);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null) _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> ExportToExcel()
    {
        var vehicles = await _context.Vehicles.OrderBy(v => v.Name).ToListAsync();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Vehicles");
        worksheet.Cells.LoadFromCollection(vehicles, true);
        
        var stream = new MemoryStream();
        await package.SaveAsAsync(stream);
        stream.Position = 0;
        
        var excelName = $"Vehicles-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    }
}