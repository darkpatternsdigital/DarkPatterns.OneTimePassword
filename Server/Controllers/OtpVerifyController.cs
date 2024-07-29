


using DarkPatterns.OneTimePassword.Auth;
using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Persistence;

namespace DarkPatterns.OneTimePassword.Controllers;

public class OtpVerifyController(
	IDeliveryMethodFactory deliveryMethodFactory
) : OtpVerifyControllerBase
{
	protected override async Task<VerifyOtpActionResult> VerifyOtp(VerifyOtpRequest verifyOtpBody)
	{
		var deliveryMethod = deliveryMethodFactory.Create(verifyOtpBody.Medium);
		if (!deliveryMethod.IsValidDestination(verifyOtpBody.Destination))
			return VerifyOtpActionResult.BadRequest();
		if (!User.TryGetApplicationId(out var appId))
			return VerifyOtpActionResult.BadRequest();

		var verifyResult = await new VerifyOtpCommand(
			verifyOtpBody.Medium,
			verifyOtpBody.Destination,
			verifyOtpBody.Otp,
			applicationId: appId
		).Execute(HttpContext);

		if (verifyResult)
			return VerifyOtpActionResult.Ok();
		else
			return VerifyOtpActionResult.Conflict();
	}
}
