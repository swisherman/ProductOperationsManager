namespace ProductOperationsManager.Options;

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "ProductOperationsDemo";
    public string ProductsCollection { get; set; } = "products";
}
