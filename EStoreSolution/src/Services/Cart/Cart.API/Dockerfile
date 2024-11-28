# Use .NET 8 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (Cart and Contracts) for dependency resolution
COPY ["src/Services/Cart/Cart.API/Cart.API.csproj", "Services/Cart/Cart.API/"]
COPY ["src/Services/Cart/Cart.Application/Cart.Application.csproj", "Services/Cart/Cart.Application/"]
COPY ["src/Services/Cart/Cart.Domain/Cart.Domain.csproj", "Services/Cart/Cart.Domain/"]
COPY ["src/Services/Cart/Cart.Persistence/Cart.Persistence.csproj", "Services/Cart/Cart.Persistence/"]

COPY ["src/Contracts/Contracts/Contracts.csproj", "Contracts/Contracts/"]

# Restore dependencies for Cart.API
RUN dotnet restore "Services/Cart/Cart.API/Cart.API.csproj"

# Copy all source code (Cart and Contracts)
COPY src/Services/Cart/Cart.API/ Services/Cart/Cart.API/
COPY src/Services/Cart/Cart.Application/ Services/Cart/Cart.Application/
COPY src/Services/Cart/Cart.Domain/ Services/Cart/Cart.Domain/
COPY src/Services/Cart/Cart.Persistence/ Services/Cart/Cart.Persistence/

COPY src/Contracts/Contracts/ Contracts/Contracts/

WORKDIR "/src/Services/Cart/Cart.API"

# Build and publish the application
RUN dotnet publish "Cart.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cart.API.dll"]