


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

		bool verifyResult = await db.VerifyOtpAsync(
			verifyOtpBody.Medium,
			verifyOtpBody.Destination,
			verifyOtpBody.Otp
		);

		if (verifyResult)
			return VerifyOtpActionResult.Ok();
		else
			return VerifyOtpActionResult.Conflict();
	}
}
