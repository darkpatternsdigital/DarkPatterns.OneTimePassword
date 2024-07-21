
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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



	internal static HttpClient CreateApiClient(this WebApplicationFactory<Program> factory, string? apiKey)
	{
		var client = factory.CreateClient();
		client.BaseAddress = new Uri(client.BaseAddress!, "/api/");
		if (apiKey != null)
			client.DefaultRequestHeaders.Add("x-api-key", apiKey);
		return client;
	}
}
