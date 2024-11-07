using DataAccess.Services;
using DataAccess.Services.Interfaces;
using StrudelShop.DataAccess.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Register services for dependency injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Add controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS (optional, useful for enabling frontend access)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

// Add authentication if using JWT
//builder.Services.AddAuthentication("Bearer")
//	.AddJwtBearer("Bearer", options =>
//	{
//		options.TokenValidationParameters = new TokenValidationParameters
//		{
//			ValidateIssuer = true,
//			ValidateAudience = true,
//			ValidateLifetime = true,
//			ValidateIssuerSigningKey = true,
//			// Configure issuer, audience, and signing key here
//			// Example:
//			// ValidIssuer = builder.Configuration["Jwt:Issuer"],
//			// ValidAudience = builder.Configuration["Jwt:Audience"],
//			// IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//		};
//	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS if configured
app.UseCors("AllowAll");

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
