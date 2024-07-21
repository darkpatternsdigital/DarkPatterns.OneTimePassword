using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class OtpDbContext : DbContext
{
	public DbSet<DeliveredOneTimePassword> DeliveredPasswords { get; set; }

	public OtpDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<DeliveredOneTimePassword>(otpModel =>
		{
			otpModel.HasKey(x => new { x.ApplicationId, x.MediumCode, x.DeliveryTarget });
		});
	}
}
