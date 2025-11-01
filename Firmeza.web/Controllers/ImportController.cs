using Firmeza.Web.Data;
using Firmeza.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ImportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ImportController(ApplicationDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
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

        // 1. Read headers and create a map
        var headerMap = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var header = worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower();
            if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
            {
                headerMap[header] = col;
            }
        }

        log.AppendLine($"Processing {worksheet.Dimension.End.Row - 1} data rows...");

        // 2. Loop through data rows
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            // Helper to get value by header name
            string? GetValue(string headerName) => 
                headerMap.ContainsKey(headerName) ? worksheet.Cells[row, headerMap[headerName]].Value?.ToString()?.Trim() : null;

            // 3. Process Product Data
            var productName = GetValue("product name") ?? GetValue("product");
            if (!string.IsNullOrWhiteSpace(productName))
            {
                var productPriceStr = GetValue("price");
                if (decimal.TryParse(productPriceStr, out var productPrice))
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == productName.ToLower());
                    if (product == null)
                    {
                        _context.Products.Add(new Product { Name = productName, Price = productPrice, Stock = 0 });
                        log.AppendLine($"- Row {row}: CREATED new product '{productName}'.");
                    }
                    else
                    {
                        product.Price = productPrice;
                        log.AppendLine($"- Row {row}: UPDATED existing product '{productName}'.");
                    }
                }
                else
                {
                    log.AppendLine($"- Row {row}: SKIPPED product '{productName}' due to invalid price.");
                }
            }

            // 4. Process Customer Data
            var customerDoc = GetValue("customer document") ?? GetValue("document");
            if (!string.IsNullOrWhiteSpace(customerDoc))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Document.ToLower() == customerDoc.ToLower());
                if (customer == null)
                {
                    var customerName = GetValue("customer name") ?? GetValue("customer");
                    var customerEmail = GetValue("email");
                    if (!string.IsNullOrWhiteSpace(customerName) && !string.IsNullOrWhiteSpace(customerEmail))
                    {
                         _context.Customers.Add(new Customer { Document = customerDoc, FullName = customerName, Email = customerEmail });
                         log.AppendLine($"- Row {row}: CREATED new customer with document '{customerDoc}'.");
                    }
                    else
                    {
                        log.AppendLine($"- Row {row}: SKIPPED customer with document '{customerDoc}' due to missing name or email.");
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        log.AppendLine("\nImport process finished. Database has been updated.");

        TempData["ImportLog"] = log.ToString();
        return RedirectToAction(nameof(Index));
    }
}