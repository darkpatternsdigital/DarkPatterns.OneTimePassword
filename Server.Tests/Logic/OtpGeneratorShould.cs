
namespace DarkPatterns.OneTimePassword.Logic;

public class OtpGeneratorShould
{
	private static OtpGenerator CreateTarget()
	{
		return new OtpGenerator();
	}

	[Fact]
	public void Generate_a_reasonable_otp()
	{
		var target = CreateTarget();
		var actual = target.GenerateOtp();
		Assert.NotNull(actual);
	}
}
