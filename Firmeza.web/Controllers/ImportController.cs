using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ImportController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly Infrastructure.Persistence.ApplicationDbContext _context; // Direct context for Vehicles

    public ImportController(IProductRepository productRepository, ICustomerRepository customerRepository, Infrastructure.Persistence.ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _context = context; // Inject DbContext
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadProducts(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a file to upload.";
            return RedirectToAction(nameof(Index));
        }

        var log = new StringBuilder();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            TempData["ErrorMessage"] = "The Excel file is empty or corrupted.";
            return RedirectToAction(nameof(Index));
        }

        var headerMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var header = worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower();
            if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
            {
                headerMap[header] = col;
            }
        }

        log.AppendLine($"Processing {worksheet.Dimension.End.Row - 1} product data rows...");

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            var productName = GetValue("product name") ?? GetValue("product");
            if (!string.IsNullOrWhiteSpace(productName))
            {
                var productPriceStr = GetValue("price");
                if (decimal.TryParse(productPriceStr, out var productPrice))
                {
                    var products = await _productRepository.GetAllAsync();
                    var product = products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
                    if (product == null)
                    {
                        await _productRepository.AddAsync(new Product { Name = productName, Price = productPrice, Stock = 0 });
                        log.AppendLine($"- Row {row}: CREATED new product '{productName}'.");
                    }
                    else
                    {
                        product.Price = productPrice;
                        await _productRepository.UpdateAsync(product);
                        log.AppendLine($"- Row {row}: UPDATED existing product '{productName}'.");
                    }
                }
                else
                {
                    log.AppendLine($"- Row {row}: SKIPPED product '{productName}' due to invalid price.");
                }
            }
        }

        TempData["ImportLog"] = log.ToString();
        return RedirectToAction("Index", "Products"); // Redirect to Products list
    }

    [HttpPost]
    public async Task<IActionResult> UploadCustomers(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a file to upload.";
            return RedirectToAction(nameof(Index));
        }

        var log = new StringBuilder();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            TempData["ErrorMessage"] = "The Excel file is empty or corrupted.";
            return RedirectToAction(nameof(Index));
        }
        
        var headerMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var header = worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower();
            if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
            {
                headerMap[header] = col;
            }
        }

        log.AppendLine($"Processing {worksheet.Dimension.End.Row - 1} customer data rows...");

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            var document = GetValue("document");
            if (string.IsNullOrWhiteSpace(document))
            {
                log.AppendLine($"- Row {row}: SKIPPED due to missing document.");
                continue;
            }

            var fullName = GetValue("fullname");
            var email = GetValue("email");

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                log.AppendLine($"- Row {row}: SKIPPED customer with document '{document}' due to missing full name or email.");
                continue;
            }

            var customers = await _customerRepository.GetAllAsync();
            var customer = customers.FirstOrDefault(c => c.Document.Equals(document, StringComparison.OrdinalIgnoreCase));

            if (customer == null)
            {
                await _customerRepository.AddAsync(new Customer
                {
                    FullName = fullName,
                    Document = document,
                    Email = email,
                    PhoneNumber = GetValue("phonenumber"),
                    Address = GetValue("address")
                });
                log.AppendLine($"- Row {row}: CREATED new customer '{fullName}'.");
            }
            else
            {
                customer.FullName = fullName;
                customer.Email = email;
                customer.PhoneNumber = GetValue("phonenumber") ?? customer.PhoneNumber;
                customer.Address = GetValue("address") ?? customer.Address;
                await _customerRepository.UpdateAsync(customer);
                log.AppendLine($"- Row {row}: UPDATED existing customer '{fullName}'.");
            }
        }

        TempData["ImportLog"] = log.ToString();
        return RedirectToAction("Index", "Customers"); // Redirect to Customers list
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadVehicles(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a file to upload.";
            return RedirectToAction(nameof(Index));
        }

        var log = new StringBuilder();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            TempData["ErrorMessage"] = "The Excel file is empty or corrupted.";
            return RedirectToAction(nameof(Index));
        }

        var headerMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var header = worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower();
            if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
            {
                headerMap[header] = col;
            }
        }

        log.AppendLine($"Processing {worksheet.Dimension.End.Row - 1} vehicle data rows...");

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            var vehicleName = GetValue("name");
            if (!string.IsNullOrWhiteSpace(vehicleName))
            {
                var hourlyRateStr = GetValue("hourlyrate");
                if (decimal.TryParse(hourlyRateStr, out var hourlyRate))
                {
                    var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Name.Equals(vehicleName, StringComparison.OrdinalIgnoreCase));
                    if (vehicle == null)
                    {
                        _context.Vehicles.Add(new Vehicle
                        {
                            Name = vehicleName,
                            Description = GetValue("description") ?? string.Empty,
                            HourlyRate = hourlyRate
                        });
                        log.AppendLine($"- Row {row}: CREATED new vehicle '{vehicleName}'.");
                    }
                    else
                    {
                        vehicle.Description = GetValue("description") ?? vehicle.Description;
                        vehicle.HourlyRate = hourlyRate;
                        log.AppendLine($"- Row {row}: UPDATED existing vehicle '{vehicleName}'.");
                    }
                }
                else
                {
                    log.AppendLine($"- Row {row}: SKIPPED vehicle '{vehicleName}' due to invalid hourly rate.");
                }
            }
        }
        
        await _context.SaveChangesAsync();
        TempData["ImportLog"] = log.ToString();
        return RedirectToAction("Index", "Vehicles"); // Redirect to Vehicles list
    }
}
