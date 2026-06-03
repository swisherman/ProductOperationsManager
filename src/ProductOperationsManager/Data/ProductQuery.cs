namespace ProductOperationsManager.Data;

public class ProductQuery
{
    public string? SearchText { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
