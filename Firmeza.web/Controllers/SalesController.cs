using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Web.Documents;
using Firmeza.Web.Helpers;
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

    public async Task<IActionResult> Index(int? pageNumber)
    {
        var salesQuery = _saleRepository.GetAll();
        int pageSize = 10;
        return View(await PaginatedList<Sale>.CreateAsync(salesQuery.OrderByDescending(s => s.SaleDate), pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> Create()
    {
        var customers = await _customerRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        var model = new SaleViewModel
        {
            Customers = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName }).ToList(),
            Products = products.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Name} (${p.Price})" }).ToList(),
            ProductPrices = products.ToDictionary(p => p.Id, p => p.Price)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var totalAmount = 0m;
            var saleDetails = new List<SaleDetail>();

            foreach (var item in model.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    totalAmount += item.Quantity * product.Price;
                    saleDetails.Add(new SaleDetail { ProductId = item.ProductId, Quantity = item.Quantity });
                }
            }
            
            var sale = new Sale
            {
                CustomerId = model.CustomerId,
                SaleDate = DateTime.UtcNow,
                SaleDetails = saleDetails,
                TotalAmount = totalAmount
            };
            await _saleRepository.AddAsync(sale);
            return RedirectToAction(nameof(Index));
        }
        
        // Repopulate dropdowns if model is not valid
        var customers = await _customerRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        model.Customers = customers.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName }).ToList();
        model.Products = products.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Name} (${p.Price})" }).ToList();
        model.ProductPrices = products.ToDictionary(p => p.Id, p => p.Price);
        
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