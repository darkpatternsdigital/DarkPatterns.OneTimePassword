
using DarkPatterns.OneTimePassword.Controllers;
using DarkPatterns.OpenApiCodegen.Json.Extensions;

namespace DarkPatterns.OneTimePassword.Persistence;

public static class OtpDbContextExtensions
{
	internal static Task PersistOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp)
	{
		// TODO
		return Task.CompletedTask;
	}

	internal static Task<bool> VerifyOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp)
	{
		// TODO
		return Task.FromResult(false);
	}
}
