using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public static class ServiceRegistration
{
	internal static void RegisterNpgsqlPersistence(this IServiceCollection services, string postgresConnectionString)
	{
		services.AddDbContext<OtpDbContext>((provider, o) =>
		{
			o.UseNpgsql(postgresConnectionString);
		});
	}
}
