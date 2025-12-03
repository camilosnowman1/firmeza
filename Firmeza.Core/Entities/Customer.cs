using System.ComponentModel.DataAnnotations;

namespace Firmeza.Core.Entities;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Name must contain only letters.")]
    public string FullName { get; set; } = default!;

    [Required]
    [MaxLength(20)]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Document must be numeric.")]
    public string Document { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [MaxLength(20)]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must be numeric.")]
    public string? PhoneNumber { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}