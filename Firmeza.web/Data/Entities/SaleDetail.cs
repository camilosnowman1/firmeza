using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Web.Data.Entities;

public class SaleDetail
{
    public int Id { get; set; }

    // Foreign key for Sale
    public int SaleId { get; set; }
    [ForeignKey("SaleId")]
    public Sale Sale { get; set; } = default!;

    // Foreign key for Product
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice => Quantity * UnitPrice;
}