using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

    private class ImportSummary
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Details { get; set; } = new List<string>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Import Completed: Processed {TotalRows} rows. Success: {SuccessCount}, Errors: {ErrorCount}.");
            sb.AppendLine("Details:");
            foreach (var detail in Details)
            {
                sb.AppendLine(detail);
            }
            return sb.ToString();
        }
    }

    private IActionResult ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a file to upload.";
            return RedirectToAction(nameof(Index));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["ErrorMessage"] = "Invalid file type. Please upload an Excel file (.xlsx or .xls).";
            return RedirectToAction(nameof(Index));
        }

        return null;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadProducts(IFormFile file)
    {
        var validationResult = ValidateFile(file);
        if (validationResult != null) return validationResult;

        var summary = new ImportSummary();
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

        summary.TotalRows = worksheet.Dimension.End.Row - 1;
        
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            var productName = GetValue("product name") ?? GetValue("product");
            if (!string.IsNullOrWhiteSpace(productName))
            {
                var productPriceStr = GetValue("price");
                if (decimal.TryParse(productPriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var productPrice))
                {
                    var products = await _productRepository.GetAllAsync();
                    var product = products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
                    if (product == null)
                    {
                        await _productRepository.AddAsync(new Product { Name = productName, Price = productPrice, Stock = 0 });
                        summary.Details.Add($"- Row {row}: CREATED new product '{productName}'.");
                        summary.SuccessCount++;
                    }
                    else
                    {
                        product.Price = productPrice;
                        await _productRepository.UpdateAsync(product);
                        summary.Details.Add($"- Row {row}: UPDATED existing product '{productName}'.");
                        summary.SuccessCount++;
                    }
                }
                else
                {
                    summary.Details.Add($"- Row {row}: SKIPPED product '{productName}' due to invalid price.");
                    summary.ErrorCount++;
                }
            }
            else
            {
                 summary.Details.Add($"- Row {row}: SKIPPED due to missing product name.");
                 summary.ErrorCount++;
            }
        }

        TempData["ImportLog"] = summary.ToString();
        return RedirectToAction("Index", "Products");
    }

    [HttpPost]
    public async Task<IActionResult> UploadCustomers(IFormFile file)
    {
        var validationResult = ValidateFile(file);
        if (validationResult != null) return validationResult;

        var summary = new ImportSummary();
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

        summary.TotalRows = worksheet.Dimension.End.Row - 1;

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            var document = GetValue("document");
            if (string.IsNullOrWhiteSpace(document))
            {
                summary.Details.Add($"- Row {row}: SKIPPED due to missing document.");
                summary.ErrorCount++;
                continue;
            }

            var fullName = GetValue("fullname");
            var email = GetValue("email");

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                summary.Details.Add($"- Row {row}: SKIPPED customer with document '{document}' due to missing full name or email.");
                summary.ErrorCount++;
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
                summary.Details.Add($"- Row {row}: CREATED new customer '{fullName}'.");
                summary.SuccessCount++;
            }
            else
            {
                customer.FullName = fullName;
                customer.Email = email;
                customer.PhoneNumber = GetValue("phonenumber") ?? customer.PhoneNumber;
                customer.Address = GetValue("address") ?? customer.Address;
                await _customerRepository.UpdateAsync(customer);
                summary.Details.Add($"- Row {row}: UPDATED existing customer '{fullName}'.");
                summary.SuccessCount++;
            }
        }

        TempData["ImportLog"] = summary.ToString();
        return RedirectToAction("Index", "Customers");
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadVehicles(IFormFile file)
    {
        var validationResult = ValidateFile(file);
        if (validationResult != null) return validationResult;

        var summary = new ImportSummary();
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

        summary.TotalRows = worksheet.Dimension.End.Row - 1;

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            var vehicleName = GetValue("name");
            if (!string.IsNullOrWhiteSpace(vehicleName))
            {
                var hourlyRateStr = GetValue("hourlyrate");
                if (decimal.TryParse(hourlyRateStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var hourlyRate))
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
                        summary.Details.Add($"- Row {row}: CREATED new vehicle '{vehicleName}'.");
                        summary.SuccessCount++;
                    }
                    else
                    {
                        vehicle.Description = GetValue("description") ?? vehicle.Description;
                        vehicle.HourlyRate = hourlyRate;
                        summary.Details.Add($"- Row {row}: UPDATED existing vehicle '{vehicleName}'.");
                        summary.SuccessCount++;
                    }
                }
                else
                {
                    summary.Details.Add($"- Row {row}: SKIPPED vehicle '{vehicleName}' due to invalid hourly rate.");
                    summary.ErrorCount++;
                }
            }
            else
            {
                summary.Details.Add($"- Row {row}: SKIPPED due to missing vehicle name.");
                summary.ErrorCount++;
            }
        }
        
        await _context.SaveChangesAsync();
        TempData["ImportLog"] = summary.ToString();
        return RedirectToAction("Index", "Vehicles");
    }
}
