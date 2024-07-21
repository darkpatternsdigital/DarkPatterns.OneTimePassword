using DarkPatterns.OneTimePassword.Client;
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

public class OtpControllerShould : IDisposable
{
	private readonly DbFixture fixture;

	public OtpControllerShould()
	{
		this.fixture = new DbFixture();
	}

	private HttpClient ConfigureApplication(Medium apiMedium, string destination, string otp)
	{
		var medium = (Controllers.Medium)apiMedium;
		var mockDeliveryMethodFactory = new Mock<IDeliveryMethodFactory>(MockBehavior.Strict);
		var mockOtpGenerator = new Mock<IOtpGenerator>(MockBehavior.Strict);

		mockDeliveryMethodFactory.Setup(m => m.Create(medium).IsValidDestination(destination)).Returns(true);
		mockOtpGenerator.Setup(m => m.GenerateOtp()).Returns(otp);
		mockDeliveryMethodFactory.Setup(m => m.Create(medium).SendOtpAsync(destination, otp)).ReturnsAsync(true);

		var webApplicationFactory = BaseWebApplicationFactory.Create()
			.WithDatabase(fixture)
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddSingleton(mockDeliveryMethodFactory.Object);
					svc.AddSingleton(mockOtpGenerator.Object);
				});
			});

		var client = webApplicationFactory.CreateApiClient(apiKey: "12345");
		return client;
	}

	[Fact]
	public async Task Provide_an_otp_given_valid_configurations()
	{
		// Arrange
		var applicationId = Guid.Empty;
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var client = ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: otp
		);

		// Act
		var response = await client.SendOtp(new(medium, destination));

		// Assert
		var result = await response.Response.Content.ReadAsStringAsync();
		response.Response.EnsureSuccessStatusCode(); // Status Code 200-299
		Assert.IsType<Operations.SendOtpReturnType.Created>(response);

		using var db = new OtpDbContext(fixture.ContextOptions);
		var deliveredRecord = db.DeliveredPasswords.SingleOrDefault(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == OtpDbContextExtensions.ToMediumCode((Controllers.Medium)medium)
			&& x.DeliveryTarget == destination
		);
		Assert.NotNull(deliveredRecord);
		Assert.True(PasswordHashing.VerifyHash(deliveredRecord.PasswordHash, otp));
	}

	public void Dispose()
	{
		((IDisposable)fixture).Dispose();
	}
}
