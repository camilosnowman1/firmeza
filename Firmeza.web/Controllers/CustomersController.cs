using Firmeza.Web.Data;
using Firmeza.Web.Data.Entities;
using Firmeza.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Customers
    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        ViewData["CurrentFilter"] = searchString;

        var customers = from c in _context.Customers.AsNoTracking() select c;

        if (!string.IsNullOrEmpty(searchString))
        {
            customers = customers.Where(c => c.FullName.Contains(searchString) || c.Document.Contains(searchString));
        }

        int pageSize = 10;
        return View(await PaginatedList<Customer>.CreateAsync(customers.OrderBy(c => c.FullName), pageNumber ?? 1, pageSize));
    }

    // GET: Customers/ExportToExcel
    public async Task<IActionResult> ExportToExcel()
    {
        var customers = await _context.Customers.OrderBy(c => c.FullName).ToListAsync();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        
        var worksheet = package.Workbook.Worksheets.Add("Customers");
        
        // Add headers
        worksheet.Cells[1, 1].Value = "Id";
        worksheet.Cells[1, 2].Value = "FullName";
        worksheet.Cells[1, 3].Value = "Document";
        worksheet.Cells[1, 4].Value = "Email";
        worksheet.Cells[1, 5].Value = "PhoneNumber";
        worksheet.Cells[1, 6].Value = "Address";
        worksheet.Cells[1, 7].Value = "CreatedAt";

        // Add data
        for (int i = 0; i < customers.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = customers[i].Id;
            worksheet.Cells[i + 2, 2].Value = customers[i].FullName;
            worksheet.Cells[i + 2, 3].Value = customers[i].Document;
            worksheet.Cells[i + 2, 4].Value = customers[i].Email;
            worksheet.Cells[i + 2, 5].Value = customers[i].PhoneNumber;
            worksheet.Cells[i + 2, 6].Value = customers[i].Address;
            worksheet.Cells[i + 2, 7].Value = customers[i].CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        var stream = new MemoryStream();
        await package.SaveAsAsync(stream);
        stream.Position = 0;
        
        var fileName = $"Customers_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    // Other CRUD actions remain the same...
    // GET: Customers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    // GET: Customers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    // POST: Customers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    // GET: Customers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // POST: Customers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.Id == id);
    }
}