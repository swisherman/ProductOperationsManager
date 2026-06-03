$baseUrl = "http://localhost:5212"
$productsUrl = "$baseUrl/products"
$products = @(
    @{
        Name = "Funny Frog Shirt"
        ListingTitle = "Minimalist Frog Graphic Tee"
        Price = 19.99
        Category = "Shirts"
    },
    @{
        Name = "Retro Bear Mug"
        ListingTitle = "Vintage Bear Coffee Mug"
        Price = 14.99
        Category = "Mugs"
    },
    @{
        Name = "Neurodivergent Coin"
        ListingTitle = "Neurodivergent & Proud Coin"
        Price = 24.99
        Category = "Coins"
    },
    @{
        Name = "Raccoon Hoodie"
        ListingTitle = "Sleepy Raccoon Hoodie"
        Price = 39.99
        Category = "Hoodies"
    },
    @{
        Name = "ADHD Planner"
        ListingTitle = "Printable ADHD Planner"
        Price = 9.99
        Category = "Digital"
    }
)

foreach ($product in $products) {

    $json = $product | ConvertTo-Json

    Invoke-RestMethod `
        -Uri $productsUrl `
        -Method POST `
        -Body $json `
        -ContentType "application/json"

    Write-Host "Inserted:" $product.Name
}