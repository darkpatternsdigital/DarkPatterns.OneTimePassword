namespace DarkPatterns.OneTimePassword.Logic;

public static class ServiceRegistration
{
	internal static void RegisterLogic(this IServiceCollection services)
	{
		services.AddSingleton<IOtpGenerator, OtpGenerator>();
	}
}
