namespace DarkPatterns.OneTimePassword.Persistence;

public class DeliveredOneTimePassword
{
	public required Guid ApplicationId { get; set; }
	public required string MediumCode { get; set; }
	public required string DeliveryTarget { get; set; }
	public required byte[] PasswordHash { get; set; }
}
