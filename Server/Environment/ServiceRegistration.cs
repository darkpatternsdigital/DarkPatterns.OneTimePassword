using Microsoft.AspNetCore.HttpOverrides;

namespace DarkPatterns.OneTimePassword.Environment;

public static class ServiceRegistration
{
	internal static void RegisterEnvironment(this IServiceCollection services,
		bool isProduction,
		string environmentName,
		IConfigurationSection buildConfig,
		IConfigurationSection dataProtectionConfig)
	{
		services.AddHealthChecks();
		services.AddControllers(config =>
		{
		});
		services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.All;
			options.KnownNetworks.Clear();
			options.KnownProxies.Clear();
		});

		services.Configure<BuildOptions>(buildConfig);

		services.RegisterDataProtection(isProduction, dataProtectionConfig);
	}

	private static void RegisterDataProtection(this IServiceCollection services, bool isProduction, IConfigurationSection dataProtectionConfig)
	{
		if (isProduction)
		{
			var dataProtectionBuilder = services.AddDataProtection();
			// TODO: add protection for environment
		}
	}
}
