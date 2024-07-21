
using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

static class StartupUtils
{
	internal static async Task CheckMigrations(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<OtpDbContext>();

		await dbContext.Database.MigrateAsync();
	}

	internal static async Task RunMigrations(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<OtpDbContext>();

		if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
		{
			throw new InvalidOperationException($"Missing {dbContext.Database.GetPendingMigrations().Count()} unapplied migrations");
		}
	}
}
