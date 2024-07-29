using DarkPatterns.OneTimePassword.Controllers;

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
}
