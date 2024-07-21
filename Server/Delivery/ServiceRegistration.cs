namespace DarkPatterns.OneTimePassword.Delivery;

public static class ServiceRegistration
{
	internal static void RegisterDelivery(this IServiceCollection services)
	{
		services.AddSingleton<IDeliveryMethodFactory, DeliveryMethodFactory>();
	}
}
