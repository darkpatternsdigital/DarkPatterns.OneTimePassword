


using DarkPatterns.OneTimePassword.Auth;
using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Persistence;

namespace DarkPatterns.OneTimePassword.Controllers;

public class OtpVerifyController(
	IDeliveryMethodFactory deliveryMethodFactory,
	OtpDbContext db
) : OtpVerifyControllerBase
{
	protected override async Task<VerifyOtpActionResult> VerifyOtp(VerifyOtpRequest verifyOtpBody)
	{
		var deliveryMethod = deliveryMethodFactory.Create(verifyOtpBody.Medium);
		if (!deliveryMethod.IsValidDestination(verifyOtpBody.Destination))
			return VerifyOtpActionResult.BadRequest();
		if (!User.TryGetApplicationId(out var appId))
			return VerifyOtpActionResult.BadRequest();

		var verifyResult = await db.VerifyOtpAsync(
			verifyOtpBody.Medium,
			verifyOtpBody.Destination,
			verifyOtpBody.Otp,
			applicationId: appId
		);

		if (verifyResult)
			return VerifyOtpActionResult.Ok();
		else
			return VerifyOtpActionResult.Conflict();
	}
}
