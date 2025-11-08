namespace Firmeza.Api.Dtos;

public class RentalDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerFullName { get; set; }
    public int VehicleId { get; set; }
    public string? VehicleName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
}
