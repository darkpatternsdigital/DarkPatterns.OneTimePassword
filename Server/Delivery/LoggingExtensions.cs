
using DarkPatterns.OneTimePassword.Controllers;

namespace DarkPatterns.OneTimePassword.Delivery;

public static partial class LoggingExtensions
{
	[LoggerMessage(
		Level = LogLevel.Information,
		Message = "IsValidDestination: {Destination}@{Medium}")]
	public static partial void LogIsValidDestination(
		this ILogger logger, string medium, string destination);

	[LoggerMessage(
		Level = LogLevel.Information,
		Message = "SendOtp: {Destination}@{Medium} - {Otp}")]
	public static partial void LogSendOtp(
		this ILogger logger, string medium, string destination, string otp);
}
