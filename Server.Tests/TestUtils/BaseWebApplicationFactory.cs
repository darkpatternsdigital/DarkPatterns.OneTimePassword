
using DarkPatterns.OneTimePassword.Persistence;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DarkPatterns.OneTimePassword.TestUtils;

internal static class BaseWebApplicationFactory
{
	public static WebApplicationFactory<Program> Create()
	{
		return new WebApplicationFactory<Program>()
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.RemoveAll<IHostedService>();
				});

				web.UseEnvironment("Testing");
			});
	}

	internal static WebApplicationFactory<Program> WithDatabase(
		this WebApplicationFactory<Program> factory,
		DbFixture fixture
	)
	{
		var resultFactory = factory
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddDbContext<OtpDbContext>((options) =>
					{
						options.UseSqlite(fixture.Connection);
					});
				});
			});

		using var db = new OtpDbContext(fixture.ContextOptions);
		db.Database.EnsureDeleted();
		db.Database.EnsureCreated();

		return resultFactory;
	}

	internal static HttpClient CreateApiClient(this WebApplicationFactory<Program> factory, string? apiKey)
	{
		var client = factory.CreateClient();
		client.BaseAddress = new Uri(client.BaseAddress!, "/api/");
		if (apiKey != null)
			client.DefaultRequestHeaders.Add("x-api-key", apiKey);
		return client;
	}
}
