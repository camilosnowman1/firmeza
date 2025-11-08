using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.Dtos;

public class UpdateRentalDto
{
    [Required]
    public int CustomerId { get; set; }
    [Required]
    public int VehicleId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}
