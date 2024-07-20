

using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Logic;
using DarkPatterns.OneTimePassword.Persistence;

namespace DarkPatterns.OneTimePassword.Controllers;

public class OtpController(
	IDeliveryMethodFactory deliveryMethodFactory,
	IOtpGenerator otpGenerator,
	OtpDbContext db
) : OtpControllerBase
{
	protected override async Task<SendOtpActionResult> SendOtp(SendOtpRequest sendOtpBody)
	{
		// TODO: get API Key details for otp persistence configuration
		var deliveryMethod = deliveryMethodFactory.Create(sendOtpBody.Medium);
		if (!deliveryMethod.IsValidDestination(sendOtpBody.Destination))
			return SendOtpActionResult.BadRequest();

		var otp = otpGenerator.GenerateOtp();
		var success = await deliveryMethod.SendOtpAsync(sendOtpBody.Destination, otp);
		if (!success)
			return SendOtpActionResult.BadRequest();

		await db.PersistOtpAsync(sendOtpBody.Medium, sendOtpBody.Destination, otp);

		return SendOtpActionResult.Created();
	}
}