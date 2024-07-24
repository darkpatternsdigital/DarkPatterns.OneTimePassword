namespace DarkPatterns.OneTimePassword.Persistence;

public class ConfiguredApplication
{
	public required Guid ApplicationId { get; set; }
	public OtpApplication? Application { get; set; }
	public required Guid ConfigurationId { get; set; }
	public OtpConfiguration? Configuration { get; set; }
}
