using ProductOperationsManager.Data;
using ProductOperationsManager.Models;

namespace ProductOperationsManager.Repositories;

public interface IProductRepository
{
    Task<List<ProductItem>> GetAllAsync(ProductQuery? query = null);
    Task<ProductItem?> GetByIdAsync(string id);
    Task<ProductItem> CreateAsync(ProductItem item);
    Task<ProductItem?> UpdateAsync(string id, ProductItem item);
    Task<bool> DeleteAsync(string id);
}
