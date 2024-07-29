using DarkPatterns.OneTimePassword.Commands;
using DarkPatterns.OneTimePassword.Controllers;

using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class VerifyOtpCommand(Medium medium, string destination, string otp, Guid applicationId) : ICommand<Task<bool>, HttpContext>
{
	public async Task<bool> Execute(HttpContext context)
	{
		var db = context.RequestServices.GetRequiredService<OtpDbContext>();
		var timeProvider = context.RequestServices.GetRequiredService<TimeProvider>();
		var deliveredRecord = await db.DeliveredPasswords.FirstOrDefaultAsync(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == medium.ToMediumCode()
			&& x.DeliveryTarget == destination
		);
		if (deliveredRecord == null)
			return false;
		try
		{
			if (deliveredRecord.ExpirationTime <= timeProvider.GetUtcNow())
			{
				db.Remove(deliveredRecord);
				return false;
			}

			var isMatch = PasswordHashing.VerifyHash(deliveredRecord.PasswordHash, otp);
			if (isMatch)
			{
				db.Remove(deliveredRecord);
				return true;
			}
			else
			{
				return false;
			}
		}
		finally
		{
			await db.SaveChangesAsync();
		}
	}
}
