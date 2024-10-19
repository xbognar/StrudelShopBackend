# Use the .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the project files
COPY src/StrudelShop.API/*.csproj ./src/StrudelShop.API/
COPY src/StrudelShop.DataAccess/*.csproj ./src/StrudelShop.DataAccess/

# Restore dependencies
RUN dotnet restore ./src/StrudelShop.API/StrudelShop.API.csproj

# Copy the entire source code
COPY src/ ./src/

# Build and publish the API project
RUN dotnet publish ./src/StrudelShop.API/StrudelShop.API.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published API from the build stage
COPY --from=build-env /app/out .

# Expose port 80
EXPOSE 80

# Start the API
ENTRYPOINT ["dotnet", "StrudelShop.API.dll"]
