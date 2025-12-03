using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.Dtos;

public class UpdateCustomerDto
{
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Name must contain only letters.")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Document must be numeric.")]
    public string Document { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must be numeric.")]
    public string? PhoneNumber { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }
}