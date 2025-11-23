namespace Firmeza.Api.Dtos.V2;

public class CustomerDtoV2
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    
    // Campos nuevos en v2
    public int TotalPurchases { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public string CustomerTier => TotalPurchases >= 10 ? "Gold" : TotalPurchases >= 5 ? "Silver" : "Bronze";
}
