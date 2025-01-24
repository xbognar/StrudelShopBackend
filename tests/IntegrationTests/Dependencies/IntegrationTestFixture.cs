using GymAPI;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StrudelShop.DataAccess.DataAccess;
using System;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// Custom WebApplicationFactory for integration tests.
	/// </summary>
	public class IntegrationTestFixture : WebApplicationFactory<Program>
	{
		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.UseEnvironment("IntegrationTest");

			// Set all environment variables your code expects:
			Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
			Environment.SetEnvironmentVariable("ADMIN_USERNAME", "admin");
			Environment.SetEnvironmentVariable("ADMIN_PASSWORD", "adminPass");
			Environment.SetEnvironmentVariable("JWT_KEY", "IntegrationTestKeyMustBeAtLeast32Char!!");
			Environment.SetEnvironmentVariable("JWT_ISSUER", "IntegrationTestIssuer");
			Environment.SetEnvironmentVariable("JWT_AUDIENCE", "IntegrationTestAudience");
			Environment.SetEnvironmentVariable("JWT_TOKEN_EXPIRY_MINUTES", "60");
			Environment.SetEnvironmentVariable("CONNECTION_STRING", "FakeIntegrationConnString");

			// Let the base create the host with in-memory DB
			var host = base.CreateHost(builder);

			using var scope = host.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			SeedDataHelper.Seed(db);

			return host;
		}
	}
}
