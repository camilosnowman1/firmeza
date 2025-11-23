namespace Firmeza.Api.Dtos.V2;

public class ProductStatisticsDto
{
    public int TotalProducts { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int ProductsInStock { get; set; }
    public int ProductsOutOfStock { get; set; }
    public int LowStockProducts { get; set; }
    public ProductDtoV2? MostExpensiveProduct { get; set; }
    public ProductDtoV2? CheapestProduct { get; set; }
}
