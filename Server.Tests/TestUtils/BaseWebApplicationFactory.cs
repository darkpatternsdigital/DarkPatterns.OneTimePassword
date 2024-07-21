using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DarkPatterns.OneTimePassword.TestUtils;

internal class BaseWebApplicationFactory : WebApplicationFactory<Program>
{
	public BaseWebApplicationFactory()
	{
		WithWebHostBuilder(web =>
		{
			web.ConfigureTestServices(svc =>
			{
				svc.RemoveAll<IHostedService>();
			});
		});
	}
}
