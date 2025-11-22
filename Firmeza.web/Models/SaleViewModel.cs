using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Firmeza.Web.Models;

public class SaleViewModel
{
    public int CustomerId { get; set; }
    public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    public List<SaleDetailViewModel> Items { get; set; } = new List<SaleDetailViewModel>();
    public Dictionary<int, decimal> ProductPrices { get; set; } = new Dictionary<int, decimal>();
    public decimal TotalAmount { get; set; }
}

public class SaleDetailViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
