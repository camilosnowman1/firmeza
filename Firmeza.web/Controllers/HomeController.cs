using System.Diagnostics;
using Firmeza.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Controllers;

[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalCustomers = await _context.Users.CountAsync(), // Assuming customers are users for now
            TotalSales = 0 // Placeholder, as Sales entity is not yet defined
        };
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalSales { get; set; }
}