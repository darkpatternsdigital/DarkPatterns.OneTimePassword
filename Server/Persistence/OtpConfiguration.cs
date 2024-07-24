namespace DarkPatterns.OneTimePassword.Persistence;

public class OtpConfiguration
{
	public required Guid ConfigurationId { get; set; }

	// Expiration details
	public required int MaxAttemptCount { get; set; }
	public required TimeSpan ExpirationWindow { get; set; }
	public required bool IsSliding { get; set; }

	// TODO: OTP generation configuration
	// TODO: delivery configurations
}
