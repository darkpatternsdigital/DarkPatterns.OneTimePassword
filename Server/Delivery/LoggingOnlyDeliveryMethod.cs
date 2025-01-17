
using DarkPatterns.OneTimePassword.Controllers;

namespace DarkPatterns.OneTimePassword.Delivery;

internal class LoggingOnlyDeliveryMethod(Medium medium, ILogger<LoggingOnlyDeliveryMethod> logger) : IDeliveryMethod
{
	public bool IsValidDestination(string destination)
	{
		logger.LogIsValidDestination(medium, destination);
		return true;
	}

	public Task<bool> SendOtpAsync(string destination, string otp)
	{
		logger.LogSendOtp(medium, destination, otp);
		return Task.FromResult(true);
	}
}
