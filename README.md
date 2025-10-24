# Bookstore Management System

A simple **.NET 8 Web API** for managing books, authors, genres, and reviews.

---

## Table of Contents

-   [Features](#features)
-   [Technologies](#technologies)
-   [Setup](#setup)
-   [Database](#database)
-   [Authentication](#authentication)
-   [Running the Application](#running-the-application)
-   [Testing](#testing)
-   [Scheduled Import Task](#scheduled-import-task)
-   [API Documentation](#api-documentation)

---

## Features

-   CRUD operations for:

    -   Books
    -   Authors
    -   Genres
    -   Reviews

-   Get books endpoint with details like:

    -   Authors
    -   Genres
    -   Average review rating

-   Get top 10 books by average rating endpoint (via Dapper / raw SQL)
-   Authentication & Authorization:

    -   Roles: `Read` and `ReadWrite`
    -   Policies: `Readers` and `ReadAndWriters`

-   Scheduled import task for bulk book import (simulated from a random batch of 100k - 200k books)
-   Structured logging using Serilog
-   DTOs & AutoMapper for clean API responses

---

## Technologies

-   .NET 8
-   ASP.NET Core Web API
-   Entity Framework Core
-   Dapper
-   Quartz.NET (for scheduled jobs)
-   xUnit + Moq (for tests)
-   Serilog (structured logging)
-   Swagger

---

## Setup

1. **Clone the repository**

```bash
git clone https://github.com/gotovac/bookstore-api.git
cd BookstoreAPI
```

2. **Restore NuGet packages**

```bash
dotnet restore
```

---

## Database

1. Ensure **SQL Server** is running.
2. Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookstoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

3. Apply migrations:

```bash
dotnet ef database update -p .\src\Bookstore.Data\ -s .\src\Bookstore.Api\
```

---

## Authentication

-   JWT-based authentication
-   Roles:

    -   `Read` → can access GET endpoints
    -   `ReadWrite` → can access all endpoints

-   Use `/api/auth/login` to generate a JWT for testing

-   For testing purposes, two users are hardcoded, use `username: readonly` and `password: password` for the `Read` role and `username: admin` and `password: password` for the `ReadWrite` role

---

## Running the Application

```bash
dotnet run -p Bookstore.Api
```

-   Or just simply F5 in Visual Studio

---

## Testing

-   **Unit tests**: located in `Bookstore.UnitTests`
-   **Integration tests**: located in `Bookstore.IntegrationTests`

Run all tests:

```bash
dotnet test
```

-   Or just simply in Visual Studio go to Test -> Run All Tests

---

## Scheduled Import Task

-   Simulates importing 100,000+ books per batch
-   Runs every hour via **Quartz.NET**
-   Configured in `Bookstore.Jobs`
-   Logging provides import statistics
-   For testing purposes, you can trigger the import manually via the `POST /api/books/test-import` endpoint.
-   You can set the batch range inside of the `Execute` method in `BookImportJob` with the `BookImportService.GenerateBooks(minCount, maxCount)` method

---

## API Documentation

-   Swagger/OpenAPI available at `/swagger`
-   Endpoints include CRUD for books, authors, genres, and reviews, plus import and authentication endpoints
