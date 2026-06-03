using System.Net.Http.Json;
using ProductOperationsManager.Models;

namespace ProductOperationsManager.Services;

public class ProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http;
    }
    public async Task<bool> DeleteProductAsync(string id)
    {
        var response = await _http.DeleteAsync($"products/{id}");
        return response.IsSuccessStatusCode;
    }
    public async Task<ProductItem?> GetProductByIdAsync(string id)
    {
        return await _http.GetFromJsonAsync<ProductItem>($"products/{id}");
    }

    public async Task<bool> UpdateProductAsync(string id, ProductItem product)
    {
        var response = await _http.PutAsJsonAsync($"products/{id}", product);
        return response.IsSuccessStatusCode;
    }
    public async Task<ProductItem?> CreateProductAsync(ProductItem product)
    {
        var response = await _http.PostAsJsonAsync("products", product);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ProductItem>();
    }

    public async Task<List<ProductItem>> GetProductsAsync()
    {
        return await _http.GetFromJsonAsync<List<ProductItem>>("products")
            ?? new List<ProductItem>();
    }

    public async Task<ProductStats?> GetStatsAsync()
    {
        return await _http.GetFromJsonAsync<ProductStats>("products/stats");
    }

    public async Task<List<ProductItem>> SearchProductsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetProductsAsync();

        return await _http.GetFromJsonAsync<List<ProductItem>>(
            $"products?q={Uri.EscapeDataString(query)}")
            ?? new List<ProductItem>();
    }
}