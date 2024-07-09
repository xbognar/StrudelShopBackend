var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//// Register the repositories
//builder.Services.AddTransient<ICustomerRepository>(provider => new CustomerRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddTransient<IProductRepository>(provider => new ProductRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddTransient<IOrderRepository>(provider => new OrderRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddTransient<IOrderItemRepository>(provider => new OrderItemRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Register the services
//builder.Services.AddTransient<ICustomerService, CustomerService>();
//builder.Services.AddTransient<IProductService, ProductService>();
//builder.Services.AddTransient<IOrderService, OrderService>();
//builder.Services.AddTransient<IOrderItemService, OrderItemService>();

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
