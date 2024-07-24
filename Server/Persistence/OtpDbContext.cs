using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class OtpDbContext : DbContext
{
	public DbSet<DeliveredOneTimePassword> DeliveredPasswords { get; set; }
	public DbSet<OtpConfiguration> Configurations { get; set; }
	public DbSet<ConfiguredApplication> ConfiguredApplications { get; set; }
	public DbSet<OtpApplication> Applications { get; set; }

	public OtpDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<DeliveredOneTimePassword>(otpModel =>
		{
			otpModel.HasKey(otp => new { otp.ApplicationId, otp.MediumCode, otp.DeliveryTarget });
			otpModel.HasOne(otp => otp.Application).WithMany().HasForeignKey(x => x.ApplicationId);
		});

		modelBuilder.Entity<OtpConfiguration>(builder =>
		{
			builder.HasKey(config => new { config.ConfigurationId });
		});

		modelBuilder.Entity<ConfiguredApplication>(builder =>
		{
			builder.HasKey(joined => new { joined.ConfigurationId, joined.ApplicationId });
			builder.HasIndex(joined => joined.ApplicationId);
			builder.HasOne(joined => joined.Application).WithMany().HasForeignKey(x => x.ApplicationId);
			builder.HasOne(joined => joined.Configuration).WithMany().HasForeignKey(x => x.ConfigurationId);
		});

		modelBuilder.Entity<OtpApplication>(builder =>
		{
			builder.HasKey(app => new { app.ApplicationId });
		});
	}
}
