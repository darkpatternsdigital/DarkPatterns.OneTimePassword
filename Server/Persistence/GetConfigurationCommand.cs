using DarkPatterns.OneTimePassword.Auth;
using DarkPatterns.OneTimePassword.Commands;

using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class GetConfigurationCommand() : ICommand<Task<OtpConfiguration?>, HttpContext>
{
	public async Task<OtpConfiguration?> Execute(HttpContext context)
	{
		if (!context.User.TryGetConfigurationId(out var configId))
			return null;

		var db = context.RequestServices.GetRequiredService<OtpDbContext>();
		return await db.Configurations.FirstOrDefaultAsync(c => c.ConfigurationId == configId);
	}
}
