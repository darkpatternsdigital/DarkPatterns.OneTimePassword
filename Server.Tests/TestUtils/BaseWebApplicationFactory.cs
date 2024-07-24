
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

using Moq;

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

	internal static WebApplicationFactory<Program> WithTime(
		this WebApplicationFactory<Program> factory,
		DateTimeOffset time,
		out Mock<TimeProvider> mockTimeProvider
	)
	{
		mockTimeProvider = new Mock<TimeProvider>();
		mockTimeProvider.Setup(tp => tp.GetUtcNow()).Returns(time);
		var timeProvider = mockTimeProvider.Object;
		var resultFactory = factory
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddSingleton(timeProvider);
				});
			});

		return resultFactory;
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

	internal static WebApplicationFactory<Program> AddAppConfiguration(
		this WebApplicationFactory<Program> factory,
		DbFixture fixture,
		Guid applicationId,
		OtpConfiguration configuration
	)
	{

		using var db = new OtpDbContext(fixture.ContextOptions);
		if (db.Applications.Find(applicationId) == null)
			db.Applications.Add(new()
			{
				ApplicationId = applicationId,
			});
		db.Configurations.Add(configuration);
		db.ConfiguredApplications.Add(new() { ApplicationId = applicationId, ConfigurationId = configuration.ConfigurationId });
		db.SaveChanges();

		return factory;
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
