using Microsoft.AspNetCore.Mvc;
using ProductOperationsManager.Data;
using ProductOperationsManager.Models;
using ProductOperationsManager.Repositories;

namespace ProductOperationsManager.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;

    public ProductsController(IProductRepository products)
    {
        _products = products;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? q,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var products = await _products.GetAllAsync(new ProductQuery
        {
            SearchText = q,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        });

        return Ok(products);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetProductStats()
    {
        var products = await _products.GetAllAsync();

        var totalProducts = products.Count;
        var activeProducts = products.Count(x => x.Active);
        var inactiveProducts = products.Count(x => !x.Active);
        var totalInventory = products.Sum(x => x.InventoryQuantity);
        var averagePrice = totalProducts > 0 ? products.Average(x => x.Price) : 0;

        var categories = products
            .Where(x => !string.IsNullOrWhiteSpace(x.Category))
            .GroupBy(x => x.Category)
            .Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                ActiveCount = g.Count(x => x.Active),
                InventoryQuantity = g.Sum(x => x.InventoryQuantity)
            })
            .OrderBy(x => x.Category)
            .ToList();

        return Ok(new ProductStats
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            InactiveProducts = inactiveProducts,
            TotalInventory = totalInventory,
            AveragePrice = averagePrice,
            CategoryCount = categories.Count,
            Categories = categories
        });
    }

    [HttpGet("export/json")]
    public async Task<IActionResult> ExportProductsJson()
    {
        var products = await _products.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportProductsCsv()
    {
        var products = await _products.GetAllAsync();
        var csv = new System.Text.StringBuilder();

        csv.AppendLine("Id,Name,ListingTitle,Category,Price,InventoryQuantity,Active,Created,LastModified");

        foreach (var product in products)
        {
            csv.AppendLine(
                $"\"{EscapeCsv(product.Id)}\"," +
                $"\"{EscapeCsv(product.Name)}\"," +
                $"\"{EscapeCsv(product.ListingTitle)}\"," +
                $"\"{EscapeCsv(product.Category)}\"," +
                $"{product.Price}," +
                $"{product.InventoryQuantity}," +
                $"{product.Active}," +
                $"{product.Created:O}," +
                $"{product.LastModified:O}"
            );
        }

        return File(
            System.Text.Encoding.UTF8.GetBytes(csv.ToString()),
            "text/csv",
            "products.csv"
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        var product = await _products.GetByIdAsync(id);

        if (product == null)
            return NotFound(new { detail = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            return BadRequest("Name is required.");

        var created = await _products.CreateAsync(item);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = created.Id },
            created
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductItem item)
    {
        var updated = await _products.UpdateAsync(id, item);

        if (updated == null)
            return NotFound(new { detail = "Product not found" });

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var deleted = await _products.DeleteAsync(id);

        if (!deleted)
            return NotFound(new { detail = "Product not found" });

        return Ok(new { deleted = true });
    }

    private static string EscapeCsv(string? value)
    {
        return (value ?? string.Empty).Replace("\"", "\"\"");
    }
}
