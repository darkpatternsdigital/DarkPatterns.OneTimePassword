
namespace DarkPatterns.OneTimePassword.Delivery;

public interface IDeliveryMethod
{
	bool IsValidDestination(string destination);
	Task<bool> SendOtpAsync(string destination, string otp);
}
