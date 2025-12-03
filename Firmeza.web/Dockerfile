# Stage 1: Base image for the final runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install common native dependencies for graphics/fonts (SkiaSharp, HarfBuzzSharp)
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    fontconfig \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

# Stage 2: Build environment with the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files and the .sln file
COPY ["Firmeza.sln", "./"]
COPY ["Firmeza.web/Firmeza.web.csproj", "Firmeza.web/"]
COPY ["Firmeza.Api/Firmeza.Api.csproj", "Firmeza.Api/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]

# Restore dependencies for the entire solution
RUN dotnet restore "Firmeza.web/Firmeza.web.csproj"

# Copy the rest of the source code
COPY . .

# Build the project (specify the project to build)
WORKDIR "/src/Firmeza.web"
RUN dotnet build "Firmeza.web.csproj" -c Release -o /app/build

# Stage 3: Publish the application
FROM build AS publish
RUN dotnet publish "Firmeza.web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final image, containing only the published application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.web.dll"]
