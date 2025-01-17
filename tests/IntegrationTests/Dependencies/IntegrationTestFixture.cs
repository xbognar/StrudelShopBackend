using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StrudelShop.DataAccess.DataAccess;
using System.Linq;

namespace IntegrationTests.Dependencies
{
	public class IntegrationTestFixture : WebApplicationFactory<Program>
	{
		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Remove existing DbContext registration
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
				if (descriptor != null)
					services.Remove(descriptor);

				// Register InMemory DB
				services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseInMemoryDatabase("IntegrationTestDb");
				});

				// Build the service provider
				var sp = services.BuildServiceProvider();

				// Create a scope and seed the DB
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<ApplicationDbContext>();

					// Ensure the database is created
					db.Database.EnsureCreated();

					// Seed test data
					SeedDataHelper.Seed(db);
				}
			});

			return base.CreateHost(builder);
		}
	}
}
