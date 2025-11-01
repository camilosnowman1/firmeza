using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Web.Data.Entities;

public class Sale
{
    public int Id { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    // Foreign key for Customer
    public int CustomerId { get; set; }
    
    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; } = default!;

    // Collection of line items for this sale
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
}