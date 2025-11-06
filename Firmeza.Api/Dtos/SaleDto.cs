using System;
using System.Collections.Generic;

namespace Firmeza.Api.Dtos;

public class SaleDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public ICollection<SaleDetailDto> SaleDetails { get; set; } = new List<SaleDetailDto>();
}