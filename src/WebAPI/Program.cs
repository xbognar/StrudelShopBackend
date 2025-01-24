using DataAccess.Services;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;


namespace GymAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			// Decide in-memory vs real SQL
			if (builder.Environment.IsEnvironment("IntegrationTest"))
			{
				// Unique in-memory DB name
				var dbName = $"IntegrationTest_{Guid.NewGuid()}";
				builder.Services.AddDbContext<ApplicationDbContext>(options =>
					options.UseInMemoryDatabase(dbName));
			}
			else
			{
				var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
					?? Environment.GetEnvironmentVariable("CONNECTION_STRING");
				builder.Services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(connectionString, sqlOptions =>
					{
						sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
					}));
			}

			// Register your DI services
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<IProductImageService, ProductImageService>();
			builder.Services.AddScoped<IOrderService, OrderService>();
			builder.Services.AddScoped<IOrderItemService, OrderItemService>();
			builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

			// Add controllers w/ circular‐reference ignoring
			builder.Services.AddControllers().AddJsonOptions(opts =>
			{
				opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			});

			// Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// CORS
			builder.Services.AddCors(opts =>
			{
				opts.AddPolicy("AllowAll", p =>
				{
					p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
				});
			});

			// Set up JWT from environment
			string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
			string? jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
			string? jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(opts =>
				{
					opts.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtIssuer,
						ValidAudience = jwtAudience,
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(jwtKey ?? "fallback_key_please_set_env")
						)
					};
				});

			var app = builder.Build();

			// If not IntegrationTest, do your “wait for SQL + migrate”
			if (!app.Environment.IsEnvironment("IntegrationTest"))
			{
				using var scope = app.Services.CreateScope();
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				for (int i = 0; i < 5; i++)
				{
					try
					{
						dbContext.Database.Migrate();
						break;
					}
					catch (SqlException)
					{
						Console.WriteLine("Waiting for SQL Server...");
						Thread.Sleep(10000);
					}
				}
			}

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseCors("AllowAll");
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.Run();

		}
	}
}