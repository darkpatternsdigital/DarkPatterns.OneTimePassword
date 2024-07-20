

namespace DarkPatterns.OneTimePassword.Controllers;

public class OtpController : OtpControllerBase
{
	protected override Task<SendOtpActionResult> SendOtp(SendOtpRequest sendOtpBody)
	{
		return Task.FromResult(SendOtpActionResult.BadRequest());
	}
}
