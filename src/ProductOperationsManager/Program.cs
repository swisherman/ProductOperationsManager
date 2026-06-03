using ProductOperationsManager.Options;
using ProductOperationsManager.Repositories;
using ProductOperationsManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDb"));

var databaseProvider = builder.Configuration["Database:Provider"] ?? "MongoDb";

if (databaseProvider.Equals("MongoDb", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IProductRepository, MongoProductRepository>();
}
else
{
    throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient<ProductService>(client =>
{
    var baseUrl = builder.Configuration["App:BaseUrl"] ?? "http://localhost:5212/";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
