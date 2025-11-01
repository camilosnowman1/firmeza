using Firmeza.Web.Data;
using Firmeza.Web.Data.Entities;
using Firmeza.Web.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Web.Controllers;

// ViewModel for the Create Sale view
public class SaleViewModel
{
    public int CustomerId { get; set; }
    public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    public List<SaleDetailViewModel> Items { get; set; } = new List<SaleDetailViewModel>();
}

public class SaleDetailViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

[Authorize(Roles = "Admin")]
public class SalesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SalesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
            
        return View(sales);
    }

    // GET: Sales/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = new SaleViewModel
        {
            Customers = await _context.Customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            }).ToListAsync(),
            Products = await _context.Products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToListAsync()
        };
        return View(viewModel);
    }

    // POST: Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleViewModel model)
    {
        if (model.CustomerId == 0 || !model.Items.Any())
        {
            TempData["ErrorMessage"] = "Please select a customer and add at least one product.";
            return RedirectToAction(nameof(Create));
        }

        var sale = new Sale
        {
            CustomerId = model.CustomerId,
            SaleDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        decimal totalAmount = 0;

        foreach (var item in model.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null) { continue; }

            var saleDetail = new SaleDetail
            {
                Sale = sale,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };
            
            totalAmount += saleDetail.TotalPrice;
            sale.SaleDetails.Add(saleDetail);
        }

        sale.TotalAmount = totalAmount;

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        // --- PDF Generation with Error Handling ---
        try
        {
            var saleWithDetails = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == sale.Id);

            if (saleWithDetails != null)
            {
                var document = new SaleInvoiceDocument(saleWithDetails);
                var pdfBytes = document.GeneratePdf();

                var receiptsDir = Path.Combine(_webHostEnvironment.WebRootPath, "receipts");
                if (!Directory.Exists(receiptsDir)) Directory.CreateDirectory(receiptsDir);

                var filePath = Path.Combine(receiptsDir, $"receipt_{sale.Id}.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
            }
        }
        catch (Exception ex)
        {
            // If PDF generation fails, store the error message to show it in the UI
            TempData["PdfGenerationError"] = $"Sale was created successfully, but failed to generate PDF. Error: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}