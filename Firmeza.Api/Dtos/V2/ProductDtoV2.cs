namespace Firmeza.Api.Dtos.V2;

public class ProductDtoV2
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    
    // Campos calculados nuevos en v2
    public bool IsAvailable => Stock > 0;
    public string StockStatus => Stock < 10 ? "Low" : Stock < 50 ? "Medium" : "High";
    public DateTime CreatedAt { get; set; }
    public int TotalSales { get; set; }
}
