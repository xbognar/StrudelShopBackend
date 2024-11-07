
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

![Model Relations](https://github.com/xbognar/StrudelShopBackend/blob/master/docs/ModelRelations.png)

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
│       └── Program.cs
|
├── StartBE.bat
├── StopBE.bat
├── docker-compose.yaml
└── Dockerfile
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
     CONNECTION_STRING="Server=strudelshop_db;Database=StrudelShop;User Id=sa;Password=YourStrongPasswordHere;"
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

## Entity Framework Core Integration

Entity Framework Core is used extensively across the DataAccess project to handle database interactions, replacing the need for manual SQL queries by enabling strongly-typed, LINQ-based data manipulation. The `ApplicationDbContext` class represents the database context, mapping models to SQL tables.

## Conclusion

StrudelShop Backend provides a robust, scalable solution for e-commerce applications, leveraging modern technologies and best practices. With its clear separation of concerns, efficient data access using Entity Framework Core, and a well-defined database schema, it is poised for future growth and enhancements.
