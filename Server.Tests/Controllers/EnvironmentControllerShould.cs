using DarkPatterns.OneTimePassword.Client;
using DarkPatterns.OneTimePassword.TestUtils;

namespace DarkPatterns.OneTimePassword.Tests;

public class EnvironmentControllerShould
{
	[Fact]
	public async Task Provide_environment_details()
	{
		// Arrange
		var client = BaseWebApplicationFactory.Create()
			.WithDatabase(out _)
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
