using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.Dtos;

public class CreateSaleDto
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required for a sale.")]
    public ICollection<CreateSaleDetailDto> Items { get; set; } = new List<CreateSaleDetailDto>();
}