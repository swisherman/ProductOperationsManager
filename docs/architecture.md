# Architecture Overview

## Application Structure

The application is a Blazor-based internal operations dashboard backed by MongoDB.

## Core Components

### Pages
Blazor UI pages for:
- Product listing
- Product editing
- Product details

### Models
C# models representing MongoDB documents.

### Services
MongoDB data access and CRUD operations.

### MongoDB
Stores:
- Products
- Related operational data

## Workflow

```text
Blazor UI
    ↓
Service Layer
    ↓
MongoDB Driver
    ↓
MongoDB Database
```

## Design Goals

- Simple internal-tool workflow
- Fast CRUD operations
- Clean separation between UI and data access
- Easily extensible for additional databases/services