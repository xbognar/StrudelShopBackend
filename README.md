
# StrudelShop Backend

The StrudelShop Backend is a comprehensive backend solution designed for managing customer orders, products, and other related entities for a shopping platform. This backend system is developed using ASP.NET Core Web API and utilizes Entity Framework Core for data access, providing a robust and efficient approach to interacting with the SQL Server database.

## Technologies Used

- **ASP.NET Core Web API**: Framework for building scalable and high-performance web APIs.
- **Entity Framework Core**: An ORM for .NET, providing a structured approach to database access.
- **SQL Server**: A relational database management system for storing and managing data.
- **Swagger**: A tool for API documentation and testing.
- **Docker**: Containerization of both the API and the database for ease of deployment and portability.

## UML Diagram

Below is the UML diagram showing the relationships between the classes:

![Model Relations](https://github.com/xbognar/StrudelShopBackend/blob/master/docs/TableRelations.png)

## Project Structure

The solution consists of two main projects:

1. **StrudelShop.WebAPI**: This is the ASP.NET Core Web API project that contains the controllers for handling HTTP requests and responses.

2. **StrudelShop.DataAccess**: A class library project that contains data models, DTOs (Data Transfer Objects), repositories, and services. This project interacts with the database using Entity Framework Core.

### Full Folder Structure

```
StrudelShopBackend/
|
├── src/
│   ├── DataAccess/
│   │   ├── DataAccess/
│   │   │   ├── ApplicationDbContext.cs
│   │   ├── DTOs/
│   │   │   ├── CustomerOrderSummaryDTO.cs
│   │   │   ├── LoginRequestDTO.cs
│   │   │   ├── LoginResponseDTO.cs
│   │   │   ├── OrderDetailsDTO.cs
│   │   │   ├── OrderHistoryDTO.cs
│   │   │   ├── ProductOverviewDTO.cs
│   │   │   ├── TopProductDTO.cs
│   │   │   └── TopSellingProductDTO.cs
│   │   ├── Models/
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── Product.cs
│   │   │   ├── ProductImage.cs
│   │   │   └── User.cs
│   │   ├── Services/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IAuthenticationService.cs
│   │   │   │   ├── IOrderItemService.cs
│   │   │   │   ├── IOrderService.cs
│   │   │   │   ├── IProductImageService.cs
│   │   │   │   ├── IProductService.cs
│   │   │   │   └── IUserService.cs
│   │   │   ├── AuthenticationService.cs
│   │   │   ├── OrderItemService.cs
│   │   │   ├── OrderService.cs
│   │   │   ├── ProductImageService.cs
│   │   │   ├── ProductService.cs
│   │   │   └── UserService.cs
│   └── WebAPI/
│       ├── Controllers/
│       │   ├── AuthenticationController.cs
│       │   ├── OrderController.cs
│       │   ├── OrderItemController.cs
│       │   ├── ProductController.cs
│       │   ├── ProductImageController.cs
│       │   └── UserController.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Dockerfile
│       └── Program.cs
├── tests/
│   ├── IntegrationTests/
│   │   ├── Controllers/
│   │   │   ├── AuthenticationControllerIntegrationTests.cs
│   │   │   ├── OrderControllerIntegrationTests.cs
│   │   │   ├── OrderItemControllerIntegrationTests.cs
│   │   │   ├── ProductControllerIntegrationTests.cs
│   │   │   ├── ProductImageControllerIntegrationTests.cs
│   │   │   └── UserControllerIntegrationTests.cs
│   │   ├── Dependencies/
│   │   │   ├── IntegrationTestFixture.cs
│   │   │   ├── TestUtilities.cs
│   │   │   └── SeedDataHelper.cs
│   │   └── IntegrationTests.csproj
│   └── UnitTests/
│       ├── Services/
│       │   ├── AuthenticationServiceTests.cs
│       │   ├── OrderServiceTests.cs
│       │   ├── ProductServiceTests.cs
│       │   ├── UserServiceTests.cs
│       ├── Controllers/
│       │   ├── AuthenticationControllerTests.cs
│       │   ├── OrderControllerTests.cs
│       │   ├── ProductControllerTests.cs
│       │   └── UserControllerTests.cs
│       └── UnitTests.csproj
│
├── StartBE.bat
├── StopBE.bat
└── docker-compose.yaml
```

## Docker Setup

The backend application is containerized using Docker, which simplifies deployment and ensures consistency across different environments.

- **Dockerfile**: Builds an image for the API, including the Data Access Layer.
- **docker-compose.yaml**: Coordinates the setup of both the API and the database containers.

## Running the Application

To run the application locally, follow these steps:

1. **Clone the Repository**: 
   ```bash
   git clone https://github.com/xbognar/StrudelShopBackend.git
   cd StrudelShopBackend
   ```

2. **Set Up the Environment**: 
   Ensure that you have a `.env` file in the project root directory. This file should contain the following environment variables:
     ```bash
     SA_PASSWORD=YourStrongPasswordHere
     CONNECTION_STRING="Server=mssql;Database=StrudelShop;User Id=sa;Password=YourStrongPasswordHere;"
     JWT_KEY=YourJwtKeyHere
     AUTH_USERNAME=YourAuthUsername
     AUTH_PASSWORD=YourAuthPassword
     ```
     Replace placeholders with your actual values.
     
3. **Start the Backend**:
   Use the provided batch script to start the backend services:
   ```bash
   StartBE.bat
   ```

4. **Stop the Backend**:
   Use the provided batch script to stop the backend services:
   ```bash
   StopBE.bat
   ```

## API Endpoints

### User Controller

- `GET /api/user`: Retrieves all users.
- `GET /api/user/{id}`: Retrieves a user by ID.
- `POST /api/user`: Creates a new user.
- `PUT /api/user/{id}`: Updates an existing user.
- `DELETE /api/user/{id}`: Deletes a user by ID.

### Orders Controller

- `GET /api/orders`: Retrieves all orders.
- `GET /api/orders/{id}`: Retrieves an order by ID.
- `POST /api/orders`: Creates a new order.
- `PUT /api/orders/{id}`: Updates an existing order.
- `DELETE /api/orders/{id}`: Deletes an order by ID.
- `GET /api/orders/history/{userId}`: Retrieves order history for a specific user.
- `GET /api/orders/details/{id}`: Retrieves detailed order information.
- `GET /api/orders/summary`: Retrieves summarized orders for admin dashboard.

### OrderItems Controller

- `GET /api/orderitems`: Retrieves all order items.
- `GET /api/orderitems/{id}`: Retrieves an order item by ID.
- `POST /api/orderitems`: Adds a new order item.
- `PUT /api/orderitems/{id}`: Updates an existing order item.
- `DELETE /api/orderitems/{id}`: Deletes an order item by ID.

### Products Controller

- `GET /api/products`: Retrieves all products.
- `GET /api/products/{id}`: Retrieves a product by ID.
- `POST /api/products`: Creates a new product.
- `PUT /api/products/{id}`: Updates an existing product.
- `DELETE /api/products/{id}`: Deletes a product by ID.
- `GET /api/products/top-selling`: Retrieves top-selling products.
- `GET /api/products/overview`: Retrieves an overview of products for admin management.

### ProductImages Controller

- `GET /api/productimages`: Retrieves all product images.
- `GET /api/productimages/{id}`: Retrieves a product image by ID.
- `POST /api/productimages`: Adds a new product image.
- `PUT /api/productimages/{id}`: Updates an existing product image.
- `DELETE /api/productimages/{id}`: Deletes a product image by ID.

### Authentication Controller

- `POST /api/authentication/login`: Authenticates a user and returns a JWT token.

## Testing

The StrudelShop Backend is backed by a thorough **testing strategy** that includes **Unit Tests** and **Integration Tests**:

1. **Unit Tests**  
   - **Tools & Frameworks**: 
     - [xUnit](https://xunit.net/) – Test framework  
     - [Moq](https://github.com/moq/moq4) – Mocking library  
     - [FluentAssertions](https://fluentassertions.com/) – Expressive, readable assertions  

   - **Scope**:
     - **Services** (e.g., `UserService`, `ProductService`, etc.) to verify logic in isolation, mocking out `DbContext` and data dependencies.
     - **Controllers** (e.g., `UserController`, `OrderController`) to confirm each endpoint’s return type (`Ok`, `NotFound`, etc.) and how they interact with service interfaces.

   - **Key Benefits**:
     - Fast feedback on core logic  
     - Ensures controllers return correct HTTP responses  
     - Verifies service-layer methods handle data consistently  

2. **Integration Tests**  
   - **Tools & Frameworks**:
     - [Microsoft.AspNetCore.Mvc.Testing](https://learn.microsoft.com/en-us/dotnet/core/testing/integration-testing) – Spins up an **in-memory** ASP.NET Core test server  
     - [EF Core InMemory](https://learn.microsoft.com/en-us/ef/core/providers/in-memory) – Replaces the SQL Server with an in-memory DB for testing  

   - **Scope**:
     - Tests full end-to-end, from the **HTTP request** through **controllers**, **services**, **entity framework layer**, and into an **in-memory** database.
     - Verifies authentication (JWT-based) and role-based endpoints.

   - **Approach**:
     1. A **`WebApplicationFactory<Program>`** loads the real `Program.cs` pipeline.  
     2. The **DbContext** is overridden to use **InMemoryDatabase**.  
     3. **Seed data** is inserted automatically for realistic test scenarios.  
     4. **`HttpClient`** calls the real endpoints (`/api/product`, `/api/order`, etc.).  
     5. Responses are validated for correct status codes, JSON payloads, and DB changes.

   - **Key Benefits**:
     - Confidence that the entire ASP.NET Core pipeline (routing, model binding, controllers, data access) behaves correctly.
     - Catches configuration or dependency injection issues.
     - Ensures authentication/authorization is enforced.

**Test Project Structure**  
The repository contains two distinct test projects in the `tests/` directory:
- **`UnitTests/`** – Contains separate folders for Controllers and Services, each with xUnit tests that mock dependencies.  
- **`IntegrationTests/`** – Contains test fixtures, seed data helpers, and full controller-level tests that run against an in-memory test server.

By combining **unit** and **integration** testing, StrudelShop Backend maintains high reliability, quickly identifies regressions, and ensures the codebase is ready for production deployment.
