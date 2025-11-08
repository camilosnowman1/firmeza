using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Firmeza.Web.Models;

public class RentalViewModel
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int VehicleId { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);

    public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Vehicles { get; set; } = new List<SelectListItem>();
    
    // This will hold the rates for JavaScript calculation
    public Dictionary<int, decimal> VehicleRates { get; set; } = new Dictionary<int, decimal>();
}