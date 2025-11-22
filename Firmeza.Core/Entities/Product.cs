using System.ComponentModel.DataAnnotations;

namespace Firmeza.Core.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}