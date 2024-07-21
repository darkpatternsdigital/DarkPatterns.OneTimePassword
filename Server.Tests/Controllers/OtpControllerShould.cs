using DarkPatterns.OneTimePassword.Api;
using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Logic;
using DarkPatterns.OneTimePassword.Persistence;
using DarkPatterns.OneTimePassword.TestUtils;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace DarkPatterns.OneTimePassword.Tests;

public class OtpControllerShould
{
	private static HttpClient ConfigureApplication(Controllers.Medium medium, string destination)
	{
		var otp = "123456";
		var mockDeliveryMethodFactory = new Mock<IDeliveryMethodFactory>(MockBehavior.Strict);
		var mockOtpGenerator = new Mock<IOtpGenerator>(MockBehavior.Strict);
		var mockOtpContext = new Mock<OtpDbContext>(MockBehavior.Strict);

		mockDeliveryMethodFactory.Setup(m => m.Create(medium).IsValidDestination(destination)).Returns(true);
		mockOtpGenerator.Setup(m => m.GenerateOtp()).Returns(otp);
		mockDeliveryMethodFactory.Setup(m => m.Create(medium).SendOtpAsync(destination, otp)).ReturnsAsync(true);

		var webApplicationFactory = BaseWebApplicationFactory.Create()
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddSingleton(mockDeliveryMethodFactory.Object);
					svc.AddSingleton(mockOtpGenerator.Object);
					svc.AddSingleton(mockOtpContext.Object);
				});
			});

		var client = webApplicationFactory.CreateApiClient(apiKey: "12345");
		return client;
	}

	[Fact]
	public async Task Provide_an_otp_given_valid_configurations()
	{
		// Arrange
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var client = ConfigureApplication((Controllers.Medium)medium, destination);

		// Act
		var response = await client.SendOtp(new(medium, destination));

		// Assert
		var result = await response.Response.Content.ReadAsStringAsync();
		response.Response.EnsureSuccessStatusCode(); // Status Code 200-299
		Assert.IsType<Operations.SendOtpReturnType.Created>(response);
	}
}
