# Use .NET 8 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (Catalog and Contracts) for dependency resolution
COPY ["src/Services/Catalog/Catalog.API/Catalog.API.csproj", "Services/Catalog/Catalog.API/"]
COPY ["src/Services/Catalog/Catalog.Application/Catalog.Application.csproj", "Services/Catalog/Catalog.Application/"]
COPY ["src/Services/Catalog/Catalog.Domain/Catalog.Domain.csproj", "Services/Catalog/Catalog.Domain/"]
COPY ["src/Services/Catalog/Catalog.Persistence/Catalog.Persistence.csproj", "Services/Catalog/Catalog.Persistence/"]

COPY ["src/Contracts/Contracts/Contracts.csproj", "Contracts/Contracts/"]

# Restore dependencies for Catalog.API
RUN dotnet restore "Services/Catalog/Catalog.API/Catalog.API.csproj"

# Copy all source code (Catalog and Contracts)
COPY src/Services/Catalog/Catalog.API/ Services/Catalog/Catalog.API/
COPY src/Services/Catalog/Catalog.Application/ Services/Catalog/Catalog.Application/
COPY src/Services/Catalog/Catalog.Domain/ Services/Catalog/Catalog.Domain/
COPY src/Services/Catalog/Catalog.Persistence/ Services/Catalog/Catalog.Persistence/

COPY src/Contracts/Contracts/ Contracts/Contracts/

WORKDIR "/src/Services/Catalog/Catalog.API"

# Build and publish the application
RUN dotnet publish "Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]