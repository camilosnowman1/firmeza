using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;

    public CustomersController(ICustomerRepository customerRepository, IEmailService emailService)
    {
        _customerRepository = customerRepository;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        ViewData["CurrentFilter"] = searchString;
        var customersQuery = _customerRepository.GetAll();

        if (!string.IsNullOrEmpty(searchString))
        {
            customersQuery = customersQuery.Where(s => s.FullName.Contains(searchString) || s.Document.Contains(searchString));
        }
        int pageSize = 10;
        return View(await PaginatedList<Customer>.CreateAsync(customersQuery.OrderBy(c => c.FullName), pageNumber ?? 1, pageSize));
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _customerRepository.AddAsync(customer);
            
            // Send welcome email
            try
            {
                await _emailService.SendEmailAsync(customer.Email, "Welcome to Firmeza!", "<h1>Thank you for registering!</h1><p>Your account has been created successfully.</p>");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }
            
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var customer = await _customerRepository.GetByIdAsync(id.Value);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _customerRepository.UpdateAsync(customer);
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var customer = await _customerRepository.GetByIdAsync(id.Value);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _customerRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ExportToExcel()
    {
        var customers = await _customerRepository.GetAllAsync();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Customers");
        worksheet.Cells.LoadFromCollection(customers, true);
        var stream = new MemoryStream();
        await package.SaveAsAsync(stream);
        stream.Position = 0;
        var excelName = $"Customers-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    }
}