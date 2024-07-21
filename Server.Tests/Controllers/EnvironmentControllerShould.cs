using DarkPatterns.OneTimePassword.Api;
using DarkPatterns.OneTimePassword.TestUtils;

namespace DarkPatterns.OneTimePassword.Tests;

public class EnvironmentControllerShould
{
	private readonly BaseWebApplicationFactory webApplicationFactory = new();

	[Fact]
	public async Task Provide_environment_details()
	{
		// Arrange
		var client = webApplicationFactory
			.CreateClient();
		client.BaseAddress = new Uri(client.BaseAddress!, "/api/");

		// Act
		var response = await client.GetEnvironmentInfo();

		// Assert
		response.Response.EnsureSuccessStatusCode(); // Status Code 200-299
		var okResult = Assert.IsType<Operations.GetEnvironmentInfoReturnType.Ok>(response);
		Assert.Equal("HEAD", okResult.Body.GitHash);
		Assert.Equal("unknown", okResult.Body.Tag);
	}
}
