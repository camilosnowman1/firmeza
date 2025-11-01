using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Web.Data.Entities;

public class Rental
{
    public int Id { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    // Foreign key for Customer
    public int CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; } = default!;

    // Foreign key for Vehicle
    public int VehicleId { get; set; }
    [ForeignKey("VehicleId")]
    public Vehicle Vehicle { get; set; } = default!;

    public double TotalHours => (EndDate - StartDate).TotalHours;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}