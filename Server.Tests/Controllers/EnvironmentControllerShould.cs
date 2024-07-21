using DarkPatterns.OneTimePassword.Client;
using DarkPatterns.OneTimePassword.TestUtils;

namespace DarkPatterns.OneTimePassword.Tests;

public class EnvironmentControllerShould : IDisposable
{
	private readonly DbFixture fixture;

	public EnvironmentControllerShould()
	{
		this.fixture = new DbFixture();
	}

	public void Dispose()
	{
		((IDisposable)fixture).Dispose();
	}

	[Fact]
	public async Task Provide_environment_details()
	{
		// Arrange
		var client = BaseWebApplicationFactory.Create()
			.WithDatabase(fixture)
			.CreateApiClient(apiKey: null);

		// Act
		var response = await client.GetEnvironmentInfo();

		// Assert
		response.Response.EnsureSuccessStatusCode(); // Status Code 200-299
		var okResult = Assert.IsType<Operations.GetEnvironmentInfoReturnType.Ok>(response);
		Assert.Equal("HEAD", okResult.Body.GitHash);
		Assert.Equal("unknown", okResult.Body.Tag);
	}
}
