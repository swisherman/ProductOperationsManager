namespace ProductOperationsManager.Models;

public class ProductStats
{
    public int TotalProducts { get; set; }

    public int ActiveProducts { get; set; }

    public int InactiveProducts { get; set; }

    public int TotalInventory { get; set; }

    public decimal AveragePrice { get; set; }

    public int CategoryCount { get; set; }

    public List<CategoryStat> Categories { get; set; } = [];
}