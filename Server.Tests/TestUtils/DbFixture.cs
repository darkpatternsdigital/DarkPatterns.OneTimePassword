using DarkPatterns.OneTimePassword.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkPatterns.OneTimePassword.TestUtils;

public class DbFixture : IDisposable
{
	public DbFixture()
	{
		// Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
		// at the end of the test (see Dispose below).
		Connection = new SqliteConnection("Filename=:memory:");
		Connection.Open();

		ContextOptions = new DbContextOptionsBuilder<OtpDbContext>()
			.UseSqlite(Connection)
			.Options;
	}

	public SqliteConnection Connection { get; }

	public DbContextOptions<OtpDbContext> ContextOptions { get; }

	public void Dispose()
	{
		((IDisposable)Connection).Dispose();
	}
}
