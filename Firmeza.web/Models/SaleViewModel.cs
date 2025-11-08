using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Firmeza.Web.Models;

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
