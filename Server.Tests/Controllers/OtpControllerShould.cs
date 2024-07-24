using DarkPatterns.OneTimePassword.Auth;
using DarkPatterns.OneTimePassword.Client;
using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Logic;
using DarkPatterns.OneTimePassword.Persistence;
using DarkPatterns.OneTimePassword.TestUtils;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace DarkPatterns.OneTimePassword.Tests;

public class OtpControllerShould : IDisposable
{
	private readonly DbFixture fixture;
	private readonly Guid applicationId;
	private readonly Guid configurationId;
	private readonly string apiKey;
	private readonly DateTimeOffset baseTime;

	public OtpControllerShould()
	{
		this.fixture = new DbFixture();
		this.applicationId = Guid.NewGuid();
		this.configurationId = Guid.NewGuid();
		this.apiKey = ApiKeyTools.ToApiKey(applicationId, configurationId);
		this.baseTime = DateTimeOffset.UtcNow;
	}

	private HttpClient ConfigureApplicationAsync(Medium apiMedium, string destination, string otp, TimeSpan expirationWindow, int attemptCount)
	{
		var medium = (Controllers.Medium)apiMedium;
		var mockDeliveryMethodFactory = new Mock<IDeliveryMethodFactory>(MockBehavior.Strict);
		var mockOtpGenerator = new Mock<IOtpGenerator>(MockBehavior.Strict);

		mockDeliveryMethodFactory.Setup(m => m.Create(medium).IsValidDestination(destination)).Returns(true);
		mockOtpGenerator.Setup(m => m.GenerateOtp()).Returns(otp);
		mockDeliveryMethodFactory.Setup(m => m.Create(medium).SendOtpAsync(destination, otp)).ReturnsAsync(true);


		var webApplicationFactory = BaseWebApplicationFactory.Create()
			.WithDatabase(fixture)
			.WithTime(baseTime, out _)
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddSingleton(mockDeliveryMethodFactory.Object);
					svc.AddSingleton(mockOtpGenerator.Object);
				});
			})
			.AddAppConfiguration(fixture, applicationId, new()
			{
				ConfigurationId = configurationId,
				ExpirationWindow = expirationWindow,
				IsSliding = false,
				MaxAttemptCount = attemptCount,
			});

		var client = webApplicationFactory.CreateApiClient(apiKey: apiKey);
		return client;
	}

	[Fact]
	public async Task Provide_an_otp_given_valid_configurations()
	{
		// Arrange
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var expirationWindow = TimeSpan.FromHours(1);
		var attemptCount = 3;
		var client = ConfigureApplicationAsync(
			apiMedium: medium,
			destination: destination,
			otp: otp,
			expirationWindow: expirationWindow,
			attemptCount: attemptCount
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
		Assert.Equal(baseTime.Add(expirationWindow), deliveredRecord.ExpirationTime);
		Assert.Equal(attemptCount, deliveredRecord.RemainingCount);
		Assert.True(PasswordHashing.VerifyHash(deliveredRecord.PasswordHash, otp));
	}

	public void Dispose()
	{
		((IDisposable)fixture).Dispose();
	}
}
