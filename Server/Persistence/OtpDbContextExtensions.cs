using DarkPatterns.OneTimePassword.Controllers;

using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public static class OtpDbContextExtensions
{
	public static string ToMediumCode(this Medium medium) =>
		medium switch
		{
			Medium.Sms => "SMS",
			Medium.Email => "EML",
			Medium.Voice => "VOI",
			_ => throw new InvalidOperationException("Unknown medium")
		};

	internal static async Task<bool> VerifyOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp, Guid applicationId)
	{
		var deliveredRecord = await db.DeliveredPasswords.FirstOrDefaultAsync(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == ToMediumCode(medium)
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
