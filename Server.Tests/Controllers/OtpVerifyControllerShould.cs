using DarkPatterns.OneTimePassword.Client;
using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Persistence;
using DarkPatterns.OneTimePassword.TestUtils;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace DarkPatterns.OneTimePassword.Tests;

public class OtpVerifyControllerShould : IDisposable
{
	private readonly DbFixture fixture;

	public OtpVerifyControllerShould()
	{
		this.fixture = new DbFixture();
	}

	private async Task<HttpClient> ConfigureApplication(Medium apiMedium, string destination, string? otp)
	{
		var medium = (Controllers.Medium)apiMedium;
		var mockDeliveryMethodFactory = new Mock<IDeliveryMethodFactory>(MockBehavior.Strict);

		mockDeliveryMethodFactory.Setup(m => m.Create(medium).IsValidDestination(destination)).Returns(true);

		var webApplicationFactory = BaseWebApplicationFactory.Create()
			.WithDatabase(fixture)
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddSingleton(mockDeliveryMethodFactory.Object);
				});
			});

		using var db = new OtpDbContext(fixture.ContextOptions);
		if (otp != null)
		{
			db.DeliveredPasswords.Add(new()
			{
				ApplicationId = Guid.Empty, // TODO
				MediumCode = medium.ToMediumCode(),
				DeliveryTarget = destination,
				PasswordHash = OtpDbContextExtensions.GeneratePasswordHash(otp),
			});
			await db.SaveChangesAsync();
		}

		var client = webApplicationFactory.CreateApiClient(apiKey: "12345");
		return client;
	}

	[Fact]
	public async Task Verify_a_correct_otp_given_valid_configurations()
	{
		// Arrange
		var applicationId = Guid.Empty;
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: otp
		);

		// Act
		var response = await client.VerifyOtp(new(medium, destination, otp));

		// Assert
		var result = await response.Response.Content.ReadAsStringAsync();
		response.Response.EnsureSuccessStatusCode(); // Status Code 200-299
		Assert.IsType<Operations.VerifyOtpReturnType.Ok>(response);

		using var db = new OtpDbContext(fixture.ContextOptions);
		var deliveredRecord = db.DeliveredPasswords.SingleOrDefault(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == OtpDbContextExtensions.ToMediumCode((Controllers.Medium)medium)
			&& x.DeliveryTarget == destination
		);
		Assert.Null(deliveredRecord);
	}

	[Fact]
	public async Task Rejects_otp_that_has_not_been_sent()
	{
		// Arrange
		var applicationId = Guid.Empty;
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: null
		);

		// Act
		var response = await client.VerifyOtp(new(medium, destination, otp));

		// Assert
		Assert.IsType<Operations.VerifyOtpReturnType.Conflict>(response);
	}

	public void Dispose()
	{
		((IDisposable)fixture).Dispose();
	}
}
