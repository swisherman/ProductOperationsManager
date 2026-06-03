namespace ProductOperationsManager.Models;

public class CategoryStat
{
    public string Category { get; set; } = "";

    public int Count { get; set; }

    public int ActiveCount { get; set; }

    public int InventoryQuantity { get; set; }
}