using DarkPatterns.OneTimePassword.Commands;
using DarkPatterns.OneTimePassword.Controllers;

using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class PersistOtpCommand(Medium medium, string destination, string otp, Guid applicationId, OtpConfiguration configuration) : ICommand<Task, HttpContext>
{
	public async Task Execute(HttpContext context)
	{
		var db = context.RequestServices.GetRequiredService<OtpDbContext>();
		var timeProvider = context.RequestServices.GetRequiredService<TimeProvider>();
		var previous = await db.DeliveredPasswords
			.FirstOrDefaultAsync(pw =>
				pw.ApplicationId == Guid.Empty
				&& pw.MediumCode == medium.ToMediumCode()
				&& pw.DeliveryTarget == destination
			);

		var newPasswordHash = PasswordHashing.GeneratePasswordHash(otp);
		if (previous != null)
		{
			previous.PasswordHash = newPasswordHash;
		}
		else
		{
			var expiration = timeProvider.GetUtcNow() + configuration.ExpirationWindow;
			db.DeliveredPasswords.Add(new()
			{
				ApplicationId = applicationId,
				MediumCode = medium.ToMediumCode(),
				DeliveryTarget = destination,
				PasswordHash = newPasswordHash,
				ExpirationTime = expiration,
				RemainingCount = configuration.MaxAttemptCount,
			});
		}
		await db.SaveChangesAsync();
	}
}
