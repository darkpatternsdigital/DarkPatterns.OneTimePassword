namespace DarkPatterns.OneTimePassword.Persistence;

public class DeliveredOneTimePassword
{
	public required Guid ApplicationId { get; set; }
	public OtpApplication? Application { get; set; }
	public required string MediumCode { get; set; }
	public required string DeliveryTarget { get; set; }
	public required byte[] PasswordHash { get; set; }
	public required int RemainingCount { get; set; }
	public required DateTimeOffset ExpirationTime { get; set; }
}
