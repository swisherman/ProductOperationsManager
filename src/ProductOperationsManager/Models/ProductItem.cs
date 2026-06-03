using System.ComponentModel.DataAnnotations;

namespace ProductOperationsManager.Models;

public class ProductItem
{
    public string? Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string ListingTitle { get; set; } = "";

    public string Description { get; set; } = "";

    public string Category { get; set; } = "";

    public string[] Tags { get; set; } = [];

    public decimal Price { get; set; }

    public bool Active { get; set; } = true;

    public int InventoryQuantity { get; set; }

    public string ImageUrl { get; set; } = "";

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
