# Firmeza - Admin Panel

Firmeza is a web application built with ASP.NET Core Razor for managing the sales and inventory of a construction supply and industrial vehicle rental business. This administrative panel allows for the management of products, customers, and sales, including features for bulk data import/export and PDF receipt generation.

## Features

- **Dashboard:** Overview of key metrics (total products, customers, sales).
- **Product Management:** Full CRUD operations for products.
- **Customer Management:** Full CRUD operations for customers.
- **Sales Management:** Create sales with multiple items and view sales history.
- **PDF Receipt Generation:** Automatically generates a professional PDF receipt for each new sale.
- **Data Import:** Bulk import data for products and customers from Excel files.
- **Data Export:** Export product and customer lists to Excel.
- **Authentication:** Secure login system for administrators based on ASP.NET Core Identity.
- **Pagination and Searching:** All data lists are paginated and include search functionality for a better user experience.

## Technology Stack

- **Framework:** .NET 8 / ASP.NET Core MVC (Razor)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 8
- **Authentication:** ASP.NET Core Identity
- **Excel Handling:** EPPlus
- **PDF Generation:** QuestPDF
- **Deployment:** Docker

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker Desktop

### Running the Application

1.  **Start the database:**
    Navigate to the `Firmeza.web` directory and run the following command to start the PostgreSQL container in the background:
    ```sh
    docker compose up -d
    ```

2.  **Run the web application:**
    You can run the project directly from your IDE (like JetBrains Rider or Visual Studio) or by using the .NET CLI from the `Firmeza.web` directory:
    ```sh
    dotnet run
    ```

3.  **Access the application:**
    Open your web browser and navigate to the URL provided in the console (usually `https://localhost:7123` or `http://localhost:5123`).

4.  **Admin Credentials:**
    - **Username:** `admin@firmeza.dev`
    - **Password:** `Admin123!`
