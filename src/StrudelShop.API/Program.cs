using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.Services.Interfaces;
using StrudelShop.Services;
using StrudelShop.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Read connection string from environment variable
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddScoped<IDbConnection>((sp) =>
	new SqlConnection(connectionString));

// Register the repositories
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrderItemRepository, OrderItemRepository>();

// Register the services
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IOrderItemService, OrderItemService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
