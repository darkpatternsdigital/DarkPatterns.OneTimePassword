
using DarkPatterns.OneTimePassword.Controllers;
using DarkPatterns.OpenApiCodegen.Json.Extensions;

using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public static class OtpDbContextExtensions
{
	internal static async Task PersistOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp)
	{
		var previous = await db.DeliveredPasswords
			.FirstOrDefaultAsync(pw =>
				pw.ApplicationId == Guid.Empty
				&& pw.MediumCode == medium.ToMediumCode()
				&& pw.DeliveryTarget == destination
			);

		var newPasswordHash = GeneratePasswordHash(otp);
		if (previous != null)
		{
			previous.PasswordHash = newPasswordHash;
		}
		else
		{
			db.DeliveredPasswords.Add(new()
			{
				ApplicationId = Guid.Empty, // TODO
				MediumCode = medium.ToMediumCode(),
				DeliveryTarget = destination,
				PasswordHash = GeneratePasswordHash(otp)
			});
		}
		await db.SaveChangesAsync();
	}

	public static byte[] GeneratePasswordHash(this string otp)
	{
		// TODO
		return Array.Empty<byte>();
	}

	internal static bool VerifyHash(byte[] passwordHash, string otp)
	{
		// TODO
		return true;
	}

	public static string ToMediumCode(this Medium medium) =>
		medium switch
		{
			Medium.Sms => "SMS",
			Medium.Email => "EML",
			Medium.Voice => "VOI",
			_ => throw new InvalidOperationException("Unknown medium")
		};

	internal static async Task<bool> VerifyOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp)
	{
		var deliveredRecord = await db.DeliveredPasswords.SingleOrDefaultAsync(
			x => x.ApplicationId == Guid.Empty // TODO
			&& x.MediumCode == ToMediumCode(medium)
			&& x.DeliveryTarget == destination
		);
		if (deliveredRecord == null)
			return false;
		var isMatch = VerifyHash(deliveredRecord.PasswordHash, otp);
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
