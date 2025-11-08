ahora# Firmeza - Sales and Rental Management System

## Project Description

Firmeza is a comprehensive system designed for managing sales and rentals of products and vehicles. The application is built with ASP.NET Core 8, following a hexagonal architecture (also known as ports and adapters) to ensure clear separation of concerns, high maintainability, and ease of testing.

The system consists of two main components:
1.  **Firmeza.web:** A web administration application (control panel) for managing products, customers, vehicles, sales, and rentals.
2.  **Firmeza.Api:** A RESTful API that exposes the core business functionalities, allowing integration with other systems or future client applications.

## Key Features

### Web Administration Panel (Firmeza.web)
*   **Authentication and Authorization:** Secure access with administrator roles.
*   **Dynamic Dashboard:** Displays real-time statistics (Total Sales, Total Revenue, New Customers).
*   **Product Management (CRUD):** Create, Read (with search and pagination), Update, Delete products.
*   **Customer Management (CRUD):** Create, Read (with search and pagination), Update, Delete customers.
*   **Vehicle Management (CRUD):** Create, Read (with search and pagination), Update, Delete vehicles.
*   **Sales Management:** Create new sales with multiple products, view sales details.
*   **Rental Management:** Create new rentals, view rental details.
*   **Data Import/Export:**
    *   Import Products, Customers, and Vehicles from Excel files.
    *   Export Products, Customers, and Vehicles to Excel files.
*   **PDF Document Generation:**
    *   Sales Invoices in PDF format.
    *   Rental Contracts in PDF format.
*   **Dynamic Calculations:** Real-time cost estimator for vehicle creation and rentals.
*   **Localization:** Currency formatted in Colombian Pesos (COP).
*   **Email Service:** Welcome emails sent upon new customer registration.

### RESTful API (Firmeza.Api)
*   **JWT Authentication:** Secure access to API endpoints.
*   **CRUD Endpoints:** For Products, Customers, and Sales.
*   **Swagger/OpenAPI:** Interactive API documentation.

## Architecture

The project follows a **Hexagonal Architecture (Ports & Adapters)**, dividing the application into clear layers:

*   **Firmeza.Core:** Contains the core business logic, domain entities, and repository interfaces (ports). It is independent of any external technology.
*   **Firmeza.Infrastructure:** Implements the interfaces defined in `Firmeza.Core`, acting as adapters for external technologies such as Entity Framework Core (for PostgreSQL database) and the SMTP email service.
*   **Firmeza.web:** The web user interface (primary adapter) that consumes services and repositories through `Firmeza.Core` and `Firmeza.Infrastructure`.
*   **Firmeza.Api:** The API interface (primary adapter) that exposes business functionality.
*   **Firmeza.Tests:** Project dedicated to unit tests for application components.

## Technologies Used

*   **Backend:** ASP.NET Core 8
*   **Database:** PostgreSQL
*   **ORM:** Entity Framework Core 8
*   **Containers:** Docker, Docker Compose
*   **PDF Generation:** QuestPDF
*   **Excel Handling:** EPPlus
*   **Authentication:** ASP.NET Core Identity, JWT Bearer Authentication
*   **Unit Testing:** NUnit, Moq, Microsoft.EntityFrameworkCore.InMemory
*   **Object Mapping:** AutoMapper
*   **Frontend:** HTML, CSS, JavaScript, Bootstrap 5

## Getting Started

### Prerequisites

Ensure you have the following installed:
*   .NET SDK 8.0
*   Docker Desktop (includes Docker Compose)

### 1. Clone the Repository

```bash
git clone <YOUR_REPOSITORY_URL>
cd Analizador_de_Ventas_para_una_Tienda_de_electrodom-sticos/Firmeza
```

### 2. Set Up the Database with Docker Compose

From the project root (`/Firmeza`), bring up the PostgreSQL container:

```bash
docker compose up -d
```

**Note:** If you encounter an `address already in use` error (port 5433), another PostgreSQL service might be running on your machine. You can stop it with:
```bash
sudo systemctl stop postgresql
```
Then, try `docker compose up -d` again.

### 3. Apply Entity Framework Migrations and Seed Data

Once the database container is running, apply the migrations and seed initial data.

**First, ensure the `Firmeza.Tests` project compiles correctly.** If not, run `dotnet clean` and `dotnet build` from the solution root.

Then, from the project root (`/Firmeza`), execute the following commands:

```bash
# Navigate to the infrastructure project to create the migration
cd Firmeza.Infrastructure

# Apply database updates (this will create the database and tables)
dotnet ef database update --startup-project ../Firmeza.web
```

### 4. Run the Web Application (Firmeza.web)

From the project root (`/Firmeza`), navigate to the web project and run it:

```bash
cd Firmeza.web
dotnet run
```
Or, if you are using Rider, simply select `Firmeza.web` as the startup project and click the "Play" button.

The application will automatically open in your browser at `http://localhost:5168`.

### 5. Access the Admin Panel

**Administrator Credentials:**
*   **Email:** `admin@firmeza.dev`
*   **Password:** `Admin123!`

### 6. Run the API (Firmeza.Api)

From the project root (`/Firmeza`), navigate to the API project and run it:

```bash
cd Firmeza.Api
dotnet run
```
The API will be available at `https://localhost:7070` (HTTPS) and `http://localhost:5168` (HTTP). You can access the Swagger documentation at `https://localhost:7070/swagger` or `http://localhost:5168/swagger`.

### 7. Run Unit Tests

From the project root (`/Firmeza`), execute the tests:

```bash
cd Firmeza.Tests
dotnet test
```
Alternatively, in Rider, you can use the "Unit Tests" window to run all tests or specific tests.

## Basic Usage

1.  **Log In:** Access `http://localhost:5168` and log in with the administrator credentials.
2.  **Navigate:** Use the top menu to access different sections (Dashboard, Products, Customers, etc.).
3.  **CRUD:** In the Products, Customers, and Vehicles sections, you can create new records, edit existing ones, delete them, and use search/pagination.
4.  **Import/Export:** In the "Import/Export" section, you can upload Excel files to import data or download templates.
5.  **Sales and Rentals:** Create new sales and rentals, then view their details to generate invoices or contracts in PDF.

---
This `README.md` provides a solid starting point! Feel free to review it and suggest any additions or changes you deem necessary.
