using System.Net;
using System.Net.Http.Json;
using ProductOperationsManager.Models;
using ProductOperationsManager.Services;

namespace ProductOperationsManager.Tests;

[TestClass]
public sealed class ProductServiceTests
{
    [TestMethod]
    public async Task GetProductsAsync_ShouldReturnProducts_WhenApiReturnsProducts()
    {
        var products = new List<ProductItem>
        {
            new() { Id = "1", Name = "Test Product" }
        };

        var service = CreateService(HttpStatusCode.OK, products);

        var result = await service.GetProductsAsync();

        Assert.HasCount(1, result);
        Assert.AreEqual("Test Product", result[0].Name);
    }

    [TestMethod]
    public async Task CreateProductAsync_ShouldReturnProduct_WhenApiReturnsCreatedProduct()
    {
        var created = new ProductItem { Id = "1", Name = "New Product" };
        var service = CreateService(HttpStatusCode.OK, created);

        var result = await service.CreateProductAsync(new ProductItem { Name = "New Product" });

        Assert.IsNotNull(result);
        Assert.AreEqual("1", result.Id);
        Assert.AreEqual("New Product", result.Name);
    }

    [TestMethod]
    public async Task CreateProductAsync_ShouldReturnNull_WhenApiFails()
    {
        var service = CreateService(HttpStatusCode.BadRequest, null);

        var result = await service.CreateProductAsync(new ProductItem { Name = "" });

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task DeleteProductAsync_ShouldReturnTrue_WhenApiSucceeds()
    {
        var service = CreateService(HttpStatusCode.NoContent, null);

        var result = await service.DeleteProductAsync("1");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task DeleteProductAsync_ShouldReturnFalse_WhenApiFails()
    {
        var service = CreateService(HttpStatusCode.NotFound, null);

        var result = await service.DeleteProductAsync("bad-id");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenApiReturnsProduct()
    {
        var product = new ProductItem { Id = "1", Name = "Test Product" };
        var service = CreateService(HttpStatusCode.OK, product);

        var result = await service.GetProductByIdAsync("1");

        Assert.IsNotNull(result);
        Assert.AreEqual("1", result.Id);
        Assert.AreEqual("Test Product", result.Name);
    }






    [TestMethod]
    public async Task UpdateProductAsync_ShouldReturnTrue_WhenApiReturnsSuccessStatus()
    {
        var service = CreateService(HttpStatusCode.NoContent, null);

        var result = await service.UpdateProductAsync(
            "1",
            new ProductItem { Id = "1", Name = "Updated Product" });

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task UpdateProductAsync_ShouldReturnFalse_WhenApiReturnsFailureStatus()
    {
        var service = CreateService(HttpStatusCode.NotFound, null);

        var result = await service.UpdateProductAsync(
            "bad-id",
            new ProductItem { Id = "bad-id", Name = "Missing Product" });

        Assert.IsFalse(result);
    }








    [TestMethod]
    public async Task SearchProductsAsync_ShouldReturnMatchingProducts_WhenApiReturnsMatches()
    {
        var products = new List<ProductItem>
    {
        new() { Id = "1", Name = "Coffee Mug" }
    };

        var service = CreateService(HttpStatusCode.OK, products);

        var result = await service.SearchProductsAsync("coffee");

        Assert.HasCount(1, result);
        Assert.AreEqual("Coffee Mug", result[0].Name);
    }



    [TestMethod]
    public async Task SearchProductsAsync_ShouldReturnAllProducts_WhenQueryIsEmpty()
    {
        var products = new List<ProductItem>
    {
        new() { Id = "1", Name = "Coffee Mug" },
        new() { Id = "2", Name = "T-Shirt" }
    };

        var service = CreateService(HttpStatusCode.OK, products);

        var result = await service.SearchProductsAsync("");

        Assert.HasCount(2, result);
    }













    private static ProductService CreateService(HttpStatusCode statusCode, object? responseBody)
    {
        var handler = new FakeHttpMessageHandler(statusCode, responseBody);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        return new ProductService(httpClient);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly object? _responseBody;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, object? responseBody)
        {
            _statusCode = statusCode;
            _responseBody = responseBody;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode);

            if (_responseBody is not null)
            {
                response.Content = JsonContent.Create(_responseBody);
            }

            return Task.FromResult(response);
        }




    }
}