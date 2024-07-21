using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.Persistence;

public class OtpDbContext : DbContext
{
	public OtpDbContext(DbContextOptions options) : base(options)
	{
	}
}
