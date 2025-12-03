# How to Install and Run Firmeza

## Prerequisites
1. **.NET 8 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Node.js** (for the Angular client): [Download here](https://nodejs.org/)
3. **PostgreSQL**: Ensure you have a running instance or use the configured cloud database.

## 1. Unzip the Project
Extract the contents of the `.zip` file to a folder on your computer.

## 2. Database Configuration
The project is configured to use a cloud database by default. If you want to use a local database:
1. Open `Firmeza.API/appsettings.json` and `Firmeza.web/appsettings.json`.
2. Update the `DefaultConnection` string with your local PostgreSQL credentials.

## 3. Backend Setup (API & Web Panel)
Open a terminal in the root folder (`firmeza`) and run:

```bash
# Restore dependencies
dotnet restore

# Run the Web Admin Panel (http://localhost:5168)
dotnet run --project Firmeza.web/Firmeza.web.csproj

# Open a NEW terminal and run the API (http://localhost:5000)
dotnet run --project Firmeza.Api/Firmeza.Api.csproj
```

## 4. Frontend Setup (Angular Client)
Open a NEW terminal, navigate to the client folder, and run:

```bash
cd Firmeza.Client

# Install dependencies (this may take a few minutes)
npm install

# Start the client (http://localhost:4200)
npm start
```

## 5. Running Tests
To verify everything is working correctly:

```bash
dotnet test Firmeza.Tests/Firmeza.Tests.csproj
```

## 6. Option B: Run with Docker (The "Executable")
If you prefer not to install .NET or Node.js manually, you can use the included Docker script.

1.  Ensure **Docker Desktop** is installed and running.
2.  Double-click the file `RUN_WITH_DOCKER.bat` in the root folder.
3.  A terminal will open and start all services automatically.
4.  Access the apps at:
    -   **Web Panel**: http://localhost:5001
    -   **API**: http://localhost:5000
    -   **Client**: http://localhost:4200

## Troubleshooting
- If you see database errors, ensure your IP is allowed if using a cloud DB, or that your local Postgres service is running.
- If ports 5000, 5168, or 4200 are in use, you may need to close other applications.
