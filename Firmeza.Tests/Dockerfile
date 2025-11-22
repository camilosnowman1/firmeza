# Stage 1: Build and Test
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files and the .sln file
COPY ["Firmeza.sln", "./"]
COPY ["Firmeza.web/Firmeza.web.csproj", "Firmeza.web/"]
COPY ["Firmeza.Api/Firmeza.Api.csproj", "Firmeza.Api/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]
COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]

# Restore dependencies for the entire solution
RUN dotnet restore "Firmeza.sln"

# Copy the rest of the source code
COPY . .

# Build the tests project
WORKDIR "/src/Firmeza.Tests"
RUN dotnet build "Firmeza.Tests.csproj" -c Release -o /app/build

# Run tests
# If tests fail, the build will fail, which is what we want for the "tests" service in docker-compose
ENTRYPOINT ["dotnet", "test", "--logger:trx"]
