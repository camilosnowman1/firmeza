using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.Dtos;

public class UpdateSaleDetailDto
{
    // Id is optional for new items in an update, but required for existing ones
    public int? Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}