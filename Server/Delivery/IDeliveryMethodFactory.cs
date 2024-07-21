
using DarkPatterns.OneTimePassword.Controllers;
using DarkPatterns.OpenApiCodegen.Json.Extensions;

namespace DarkPatterns.OneTimePassword.Delivery;

public interface IDeliveryMethodFactory
{
	IDeliveryMethod Create(Medium medium);
}
