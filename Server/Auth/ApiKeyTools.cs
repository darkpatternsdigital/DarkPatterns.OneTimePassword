namespace DarkPatterns.OneTimePassword.Auth;

public static class ApiKeyTools
{
	public static string ToApiKey(Guid applicationId, Guid configurationId)
	{
		return Convert.ToBase64String(applicationId.ToByteArray().Concat(configurationId.ToByteArray()).ToArray());
	}

	public static ApiKeyParts? ParseApiKey(string apiKey)
	{
		try
		{
			var bytes = Convert.FromBase64String(apiKey);
			if (bytes.Length != 32) return null;
			return new ApiKeyParts(
				new Guid(bytes[0..16]),
				new Guid(bytes[16..32])
			);
		}
		catch
		{
			return null;
		}
	}
}
