# API Documentation

This application uses ASP.NET Core minimal APIs/controllers to manage product data stored in MongoDB.

---

# Base URL

```txt
http://localhost:5212
```

---
# Products API
## Get All Products

GET /products

### Example Response

```json
[
  {
    "id": "681f123456789",
    "name": "Sample Product",
    "listingTitle": "Funny Frog Shirt",
    "price": 19.99
  }
]
```

---

## Get Product By ID

```http
GET /products/{id}
```

Returns a single product.

------

## Create Product

```http
POST /products
```

### Example Request

```json
{
  "name": "New Product",
  "listingTitle": "Retro Bear Shirt",
  "price": 24.99
}
```

---

## Update Product


```http
PUT /products/{id}
```

Updates an existing product.

---

## Delete Product

```http
DELETE /products/{id}
```

Deletes a product.

## Product Stats

```http
GET /products/stats
```

## Product Json Export

```http
GET /products/export/json
```

## Product CSV Export

```http
GET /products/export/csv
```


## Query Parameters

Supported on:

- `GET /products`
- `GET /products/export/json`
- `GET /products/export/csv`

Parameters:

- `q` — text search
- `category` — filter by category
- `minPrice` — minimum product price
- `maxPrice` — maximum product price