using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Web.Documents;
using Firmeza.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SalesController : Controller
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public SalesController(ISaleRepository saleRepository, ICustomerRepository customerRepository, IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _saleRepository.GetAllAsync();
        return View(sales);
    }

    public async Task<IActionResult> Create()
    {
        var customers = await _customerRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        var model = new SaleViewModel
        {
            Customers = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName }).ToList(),
            Products = products.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var sale = new Sale
            {
                CustomerId = model.CustomerId,
                SaleDate = DateTime.UtcNow,
                SaleDetails = model.Items.Select(i => new SaleDetail { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
            };
            await _saleRepository.AddAsync(sale);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
    
    // New Details Method
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sale = await _saleRepository.GetByIdAsync(id.Value);
        if (sale == null)
        {
            return NotFound();
        }

        return View(sale);
    }
    
    // New GenerateInvoice Method
    public async Task<IActionResult> GenerateInvoice(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }

        var document = new SaleInvoiceDocument(sale);
        var pdf = document.GeneratePdf();
        return File(pdf, "application/pdf", $"SaleInvoice-{id}.pdf");
    }
}