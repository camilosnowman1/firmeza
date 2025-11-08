using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index(string searchString, int? pageNumber)
    {
        ViewData["CurrentFilter"] = searchString;
        var productsQuery = _productRepository.GetAll(); // Use the new GetAll() returning IQueryable

        if (!string.IsNullOrEmpty(searchString))
        {
            productsQuery = productsQuery.Where(s => s.Name.Contains(searchString));
        }
        int pageSize = 10;
        return View(await PaginatedList<Product>.CreateAsync(productsQuery.OrderBy(p => p.Name), pageNumber ?? 1, pageSize));
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var product = await _productRepository.GetByIdAsync(id.Value);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _productRepository.UpdateAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var product = await _productRepository.GetByIdAsync(id.Value);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}