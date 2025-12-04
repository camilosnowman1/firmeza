using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Firmeza.Web.Models;
using Firmeza.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        
        // Real database queries for dashboard
        ViewData["TotalSales"] = await _context.Sales.CountAsync();
        ViewData["TotalRevenue"] = await _context.Sales.SumAsync(s => s.TotalAmount);
        ViewData["NewCustomers"] = await _context.Customers.CountAsync(c => c.CreatedAt >= now.AddDays(-30));
        
        return View();
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
