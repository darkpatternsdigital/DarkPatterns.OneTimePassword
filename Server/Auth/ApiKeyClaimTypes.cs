using System.Security.Claims;

namespace DarkPatterns.OneTimePassword.Auth;

public static class ApiKeyClaimTypes
{
	public const string ApplicationId = "otp:appId";
	public const string ConfigurationId = "otp:configId";

	public static bool TryGetApplicationId(this ClaimsPrincipal claimsPrincipal, out Guid applicationId)
	{
		if (claimsPrincipal.FindFirst(ApplicationId) is not { Value: var s } || !Guid.TryParse(s, out applicationId))
		{
			applicationId = Guid.Empty;
			return false;
		}
		return true;
	}

	public static bool TryGetConfigurationId(this ClaimsPrincipal claimsPrincipal, out Guid configurationId)
	{
		if (claimsPrincipal.FindFirst(ConfigurationId) is not { Value: var s } || !Guid.TryParse(s, out configurationId))
		{
			configurationId = Guid.Empty;
			return false;
		}
		return true;
	}
}
