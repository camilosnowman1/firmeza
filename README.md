# Firmeza Project

Firmeza is a business management system designed for the construction sector. It includes a web administrative panel and a RESTful API to manage products, customers, sales, and rentals.

## Project Modules

The solution is organized into the following projects:

-   `Firmeza.Core`: Contains domain entities, interfaces, and core business logic.
-   `Firmeza.Infrastructure`: Implements the data access layer (repositories with Entity Framework Core), PostgreSQL database connection, and external services like email sending (SMTP).
-   `Firmeza.web`: An administrative panel built with ASP.NET Core and Razor Pages. Allows internal business management, including Excel data import.
-   `Firmeza.Api`: A RESTful API built with ASP.NET Core Web API. Exposes endpoints to be consumed by client applications (such as a future Blazor portal).
-   `Firmeza.Tests`: Unit testing project using xUnit to validate system functionality.

## Technologies Used

-   **Backend:** .NET 8, ASP.NET Core (Razor Pages & Web API)
-   **Database:** PostgreSQL
-   **ORM:** Entity Framework Core
-   **Authentication:** ASP.NET Core Identity with JSON Web Tokens (JWT) for the API.
-   **Object Mapping:** AutoMapper
-   **API Documentation:** Swashbuckle (Swagger)
-   **Excel Manipulation:** EPPlus
-   **PDF Generation:** QuestPDF
-   **Testing:** xUnit
-   **Containerization:** Docker

## Installation and Execution

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Docker](https://www.docker.com/products/docker-desktop) (Optional, for deployment)

### Database

The connection string is located in the `appsettings.json` files of the `Firmeza.web` and `Firmeza.Api` projects. Currently, it is configured to connect to a PostgreSQL instance on Clever Cloud.

If you wish to use a local database, ensure you have PostgreSQL installed and update the connection string.

### Running the Application

You can run both projects simultaneously from your IDE (Rider/Visual Studio) or via the command line:

**To run the Web Panel (Razor):**
```bash
dotnet run --project Firmeza.web/Firmeza.web.csproj
```
The application will be available at `http://localhost:5168`.

**To run the API:**
```bash
dotnet run --project Firmeza.Api/Firmeza.Api.csproj
```
The API will be available at `http://localhost:5000`.

## API Endpoints and Versioning

The API supports multiple versioning to ensure evolution without breaking compatibility.

### Version 1 (`/api/v1`)
Standard CRUD endpoints for basic operations.

### Version 2 (`/api/v2`) - **NEW!**
Enhanced version with advanced features:
-   **Products:** Advanced search, price/stock filters, inventory statistics, low stock detection.
-   **Customers:** Detailed purchase history, frequent customer identification, consumption metrics.

### Swagger Documentation
You can interactively explore and test both versions:
`http://localhost:5000/swagger`
(Use the selector in the top-right corner to switch between V1 and V2)

## Unit Testing

The project includes a robust unit testing suite using **xUnit** and **Moq**.

### Current Coverage
-   **ProductsV2Controller:** 11 tests (Filters, Statistics, Low Stock)
-   **CustomersV2Controller:** 8 tests (History, Frequent Customers)

### Running Tests
To run version 2 tests:
```bash
dotnet test Firmeza.Tests/Firmeza.Tests.csproj --filter "FullyQualifiedName~V2"
```

## Web Client (Angular)

The project includes a modern web client developed in **Angular** that consumes the API.
-   Location: `Firmeza.Client`
-   Execution: `npm start` (in the client directory)
-   URL: `http://localhost:4200`

## Credits

Developed by **Fabian Lugo**, student at **Riwi**.


