
using DarkPatterns.OneTimePassword.Controllers;

namespace DarkPatterns.OneTimePassword.Delivery;

internal class DeliveryMethodFactory(ILogger<LoggingOnlyDeliveryMethod> logger) : IDeliveryMethodFactory
{
	public IDeliveryMethod Create(Medium medium)
	{
		return new LoggingOnlyDeliveryMethod(medium, logger);
	}
}
