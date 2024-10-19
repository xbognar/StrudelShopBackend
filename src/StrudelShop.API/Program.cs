using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.Services;
using StrudelShop.DataAccess.Services.Interfaces;
using StrudelShop.Services;
using StrudelShop.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddScoped<IDbConnection>((sp) =>
	new SqlConnection(connectionString));

builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrderItemRepository, OrderItemRepository>();

builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IOrderItemService, OrderItemService>();

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var loginUsername = Environment.GetEnvironmentVariable("AUTH_USERNAME");
var loginPassword = Environment.GetEnvironmentVariable("AUTH_PASSWORD");

builder.Services.AddTransient<IAuthService, AuthService>(provider =>
	new AuthService(loginUsername, loginPassword, jwtKey));

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
		ValidateIssuer = false,
		ValidateAudience = false
	};
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
