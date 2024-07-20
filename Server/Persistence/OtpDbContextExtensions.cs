
using DarkPatterns.OneTimePassword.Controllers;
using DarkPatterns.OpenApiCodegen.Json.Extensions;

namespace DarkPatterns.OneTimePassword.Persistence;

public static class OtpDbContextExtensions
{
	internal static Task PersistOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp)
	{
		throw new NotImplementedException();
	}

	internal static Task<bool> VerifyOtpAsync(this OtpDbContext db, Medium medium, string destination, string otp)
	{
		throw new NotImplementedException();
	}
}
