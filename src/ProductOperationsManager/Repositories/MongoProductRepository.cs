using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ProductOperationsManager.Data;
using ProductOperationsManager.Models;
using ProductOperationsManager.Options;

namespace ProductOperationsManager.Repositories;

public class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductItem> _products;

    static MongoProductRepository()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(ProductItem)))
        {
            BsonClassMap.RegisterClassMap<ProductItem>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
                classMap.MapIdMember(x => x.Id)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }
    }

    public MongoProductRepository(IOptions<MongoDbOptions> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _products = database.GetCollection<ProductItem>(settings.ProductsCollection);
    }

    public async Task<List<ProductItem>> GetAllAsync(ProductQuery? query = null)
    {
        var filter = BuildFilter(query);

        return await _products
            .Find(filter)
            .SortByDescending(x => x.LastModified)
            .ToListAsync();
    }

    public async Task<ProductItem?> GetByIdAsync(string id)
    {
        return await _products
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<ProductItem> CreateAsync(ProductItem item)
    {
        item.Id = string.IsNullOrWhiteSpace(item.Id)
            ? ObjectId.GenerateNewId().ToString()
            : item.Id;

        item.Created = DateTime.UtcNow;
        item.LastModified = DateTime.UtcNow;

        await _products.InsertOneAsync(item);
        return item;
    }

    public async Task<ProductItem?> UpdateAsync(string id, ProductItem item)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null)
            return null;

        item.Id = id;
        item.Created = existing.Created;
        item.LastModified = DateTime.UtcNow;

        await _products.ReplaceOneAsync(x => x.Id == id, item);
        return item;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _products.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    private static FilterDefinition<ProductItem> BuildFilter(ProductQuery? query)
    {
        var builder = Builders<ProductItem>.Filter;
        var filters = new List<FilterDefinition<ProductItem>>();

        if (!string.IsNullOrWhiteSpace(query?.SearchText))
        {
            var searchText = query.SearchText.Trim();
            var search = builder.Or(
                builder.Regex(x => x.Name, new BsonRegularExpression(searchText, "i")),
                builder.Regex(x => x.ListingTitle, new BsonRegularExpression(searchText, "i")),
                builder.Regex(x => x.Description, new BsonRegularExpression(searchText, "i")),
                builder.Regex(x => x.Tags, new BsonRegularExpression(searchText, "i"))
            );

            filters.Add(search);
        }

        if (!string.IsNullOrWhiteSpace(query?.Category))
            filters.Add(builder.Eq(x => x.Category, query.Category));

        if (query?.MinPrice != null)
            filters.Add(builder.Gte(x => x.Price, query.MinPrice.Value));

        if (query?.MaxPrice != null)
            filters.Add(builder.Lte(x => x.Price, query.MaxPrice.Value));

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }
}
