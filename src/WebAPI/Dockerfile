# Use the .NET 8.0 SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY ["src/WebAPI/WebAPI.csproj", "src/WebAPI/"]
COPY ["src/DataAccess/DataAccess.csproj", "src/DataAccess/"]
RUN dotnet restore "src/WebAPI/WebAPI.csproj"

# Copy the rest of the source code and build the application
COPY . .
WORKDIR "/app/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

# Publish the application to the /app/publish folder
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

# Use the official .NET 8.0 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
