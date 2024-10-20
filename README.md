# StrudelShop Backend

The StrudelShop Backend is a comprehensive backend solution designed for managing customer orders, products, and other related entities for a shopping platform. This backend system is developed using ASP.NET Core Web API and utilizes Dapper for data access, providing a robust and efficient approach to interacting with the SQL Server database.

## Technologies Used

- **ASP.NET Core Web API**: Framework for building scalable and high-performance web APIs.
- **Dapper**: A simple object mapper for .NET, providing fast and efficient data access.
- **SQL Server**: A relational database management system for storing and managing data.
- **Swagger**: A tool for API documentation and testing.
- **Docker**: Containerization of both the API and the database for ease of deployment and portability.

## UML Diagram

Below is the UML diagram showing the relationships between the classes:

![Model Relations](https://github.com/xbognar/StrudelShopBackend/blob/master/docs/ModelRelations.png)

## Project Structure

The solution consists of three main projects:

1. **StrudelShop.API**: This is the ASP.NET Core Web API project which contains the controllers for handling HTTP requests and responses.

2. **StrudelShop.DataAccess**: A class library project that contains data models, DTOs (Data Transfer Objects), repositories, and services. This project interacts with the database using Dapper.

3. **StrudelShop.Database**: A SQL Server database project which contains SQL scripts for creating tables and stored procedures required for the application.

### Full Folder Structure

```
StrudelShopBackend/
|
├── src/
│   ├── StrudelShop.API/
│   │   ├── Controllers/
│   │   │   ├── CustomersController.cs
│   │   │   ├── OrdersController.cs
│   │   │   ├── OrderItemsController.cs
│   │   │   └── ProductsController.cs
│   │   ├── Properties/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   │
│   ├── StrudelShop.DataAccess/
│   │   ├── DTOs/
│   │   │   ├── OrderDTO.cs
│   │   │   ├── OrderDetailsDTO.cs
│   │   │   ├── OrderItemDTO.cs
│   │   │   └── TopSellingProductDTO.cs
│   │   ├── DataAccess/
│   │   │   ├── Interfaces/
│   │   │   │   ├── ICustomerRepository.cs
│   │   │   │   ├── IOrderRepository.cs
│   │   │   │   ├── IOrderItemRepository.cs
│   │   │   │   └── IProductRepository.cs
│   │   │   ├── CustomerRepository.cs
│   │   │   ├── OrderRepository.cs
│   │   │   ├── OrderItemRepository.cs
│   │   │   └── ProductRepository.cs
│   │   ├── Models/
│   │   │   ├── Customer.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── Product.cs
│   │   ├── Services/
│   │   │   ├── Interfaces/
│   │   │   │   ├── ICustomerService.cs
│   │   │   │   ├── IOrderService.cs
│   │   │   │   ├── IOrderItemService.cs
│   │   │   │   └── IProductService.cs
│   │   │   ├── CustomerService.cs
│   │   │   ├── OrderService.cs
│   │   │   ├── OrderItemService.cs
│   │   │   └── ProductService.cs
│   │
│   └── StrudelShop.Database/
│       ├── Tables/
│       │   ├── Customer.sql
│       │   ├── Order.sql
│       │   ├── OrderItem.sql
│       │   └── Product.sql
│       ├── StoredProcedures/
│       │   ├── Customer/
│       │   │   ├── CreateCustomer.sql
│       │   │   ├── DeleteCustomer.sql
│       │   │   ├── GetAllCustomers.sql
│       │   │   ├── GetCustomerById.sql
│       │   │   └── UpdateCustomer.sql
│       │   ├── Order/
│       │   │   ├── CreateOrder.sql
│       │   │   ├── DeleteOrder.sql
│       │   │   ├── GetAllOrders.sql
│       │   │   ├── GetCustomerOrderHistory.sql
│       │   │   ├── GetOrderById.sql
│       │   │   ├── GetOrdersByDateRange.sql
│       │   │   ├── GetTotalSalesByDateRange.sql
│       │   │   └── UpdateOrder.sql
│       │   ├── OrderItem/
│       │   │   ├── AddOrderItem.sql
│       │   │   ├── DeleteOrderItem.sql
│       │   │   ├── GetAllOrderItems.sql
│       │   │   ├── GetOrderItemById.sql
│       │   │   └── UpdateOrderItem.sql
│       │   └── Product/
│       │       ├── CreateProduct.sql
│       │       ├── DeleteProduct.sql
│       │       ├── GetAllProducts.sql
│       │       ├── GetProductById.sql
│       │       ├── GetTopSellingProducts.sql
│       │       └── UpdateProduct.sql
│       └── StrudelShop.Database.sqlproj
│
├── dbscripts/
│   ├── entrypoint.sh
│   └── StrudelShop.Database.dacpac
|
├── StartBE.bat
├── StopBE.bat
├── docker-compose.yaml
├── Dockerfile
└── Dockerfile.db
```

## Docker Setup

The backend application is containerized using Docker, which simplifies deployment and ensures consistency across different environments.

- **Dockerfile**: Builds an image for the API, including the Data Access Layer.
- **Dockerfile.db**: Builds an image for the database, applying the schema defined in the SQL project (`StrudelShop.Database`).
- **docker-compose.yaml**: Coordinates the setup of both the API and the database containers.
- **dbscripts/entrypoint.sh**: Used during the database container startup to apply the database schema from the `.dacpac` file.

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
   ```
   StartBE.bat
   ```

4. **Stop the Backend**:
   Use the provided batch script to stop the backend services:
   ```
   StopBE.bat
   ```

## API Endpoints

### Customers Controller

- `GET /api/customers`: Retrieves all customers.
- `GET /api/customers/{id}`: Retrieves a customer by ID.
- `POST /api/customers`: Creates a new customer.
- `PUT /api/customers/{id}`: Updates an existing customer.
- `DELETE /api/customers/{id}`: Deletes a customer by ID.

### Orders Controller

- `GET /api/orders`: Retrieves all orders.
- `GET /api/orders/{id}`: Retrieves an order by ID.
- `POST /api/orders`: Creates a new order.
- `PUT /api/orders/{id}`: Updates an existing order.
- `DELETE /api/orders/{id}`: Deletes an order by ID.
- `GET /api/orders/by-date-range`: Retrieves orders within a date range.
- `GET /api/orders/{id}/details`: Retrieves detailed order information.
- `GET /api/orders/total-sales`: Retrieves total sales for a date range.

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

## Dapper Integration

Dapper is a lightweight and fast object mapper for .NET, used for interacting with the database in this project. It simplifies data access by mapping the results of SQL queries to C# objects, which helps in reducing boilerplate code and improves performance. Dapper is used extensively across the repository classes to execute stored procedures and handle SQL queries efficiently.

## SQL Server Project

The `StrudelShop.Database` project contains SQL scripts for defining the database schema and stored procedures. This setup includes tables for customers, orders, order items, and products. Each table has corresponding stored procedures for CRUD operations, ensuring a clean separation between the application logic and database interactions.

## Conclusion

StrudelShop Backend provides a robust, scalable solution for e-commerce applications, leveraging modern technologies and best practices. With its clear separation of concerns, efficient data access using Dapper, and a well-defined database schema, it is poised for future growth and enhancements.
