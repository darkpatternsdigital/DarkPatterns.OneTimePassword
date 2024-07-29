using DarkPatterns.OneTimePassword.Commands;
using DarkPatterns.OneTimePassword.Controllers;

using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class VerifyOtpCommand(Medium medium, string destination, string otp, Guid applicationId) : ICommand<Task<bool>, HttpContext>
{
	public async Task<bool> Execute(HttpContext context)
	{
		var db = context.RequestServices.GetRequiredService<OtpDbContext>();
		var deliveredRecord = await db.DeliveredPasswords.FirstOrDefaultAsync(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == medium.ToMediumCode()
			&& x.DeliveryTarget == destination
		);
		if (deliveredRecord == null)
			return false;
		var isMatch = PasswordHashing.VerifyHash(deliveredRecord.PasswordHash, otp);
		if (isMatch)
		{
			db.Remove(deliveredRecord);
			await db.SaveChangesAsync();
			return true;
		}
		else
		{
			return false;
		}
	}
}
