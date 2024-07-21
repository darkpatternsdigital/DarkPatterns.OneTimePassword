
using DarkPatterns.OneTimePassword.Controllers;

namespace DarkPatterns.OneTimePassword.Delivery;

public static partial class LoggingExtensions
{
	[LoggerMessage(
		Level = LogLevel.Information,
		Message = "IsValidDestination: {Destination}@{Medium}")]
	private static partial void LogIsValidDestination(
		this ILogger logger, string medium, string destination);

	public static void LogIsValidDestination(
		this ILogger logger, Medium medium, string destination) =>
		LogIsValidDestination(logger, medium.ToString("g"), destination);

	[LoggerMessage(
		Level = LogLevel.Information,
		Message = "SendOtp: {Destination}@{Medium} - {Otp}")]
	private static partial void LogSendOtp(
		this ILogger logger, string medium, string destination, string otp);

	public static void LogSendOtp(
		this ILogger logger, Medium medium, string destination, string otp) =>
		LogSendOtp(logger, medium.ToString("g"), destination, otp);

}
