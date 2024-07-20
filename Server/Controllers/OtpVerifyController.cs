


namespace DarkPatterns.OneTimePassword.Controllers;

public class OtpVerifyController : OtpVerifyControllerBase
{
	protected override Task<VerifyOtpActionResult> VerifyOtp(VerifyOtpRequest verifyOtpBody)
	{
		return Task.FromResult(VerifyOtpActionResult.BadRequest());
	}
}
