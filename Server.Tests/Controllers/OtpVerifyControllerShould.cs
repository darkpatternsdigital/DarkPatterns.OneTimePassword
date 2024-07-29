using DarkPatterns.OneTimePassword.Auth;
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
	private readonly Guid applicationId;
	private readonly Guid configurationId;
	private readonly string apiKey;
	private readonly DateTimeOffset baseTime;
	private Mock<TimeProvider>? mockTimeProvider;

	public OtpVerifyControllerShould()
	{
		this.fixture = new DbFixture();
		this.applicationId = Guid.NewGuid();
		this.configurationId = Guid.NewGuid();
		this.apiKey = ApiKeyTools.ToApiKey(applicationId, configurationId);
		this.baseTime = DateTimeOffset.UtcNow;
	}

	private async Task<HttpClient> ConfigureApplication(Medium apiMedium, string destination, string? otp, TimeSpan expirationWindow, int attemptCount, bool isSliding = false)
	{
		var medium = (Controllers.Medium)apiMedium;
		var mockDeliveryMethodFactory = new Mock<IDeliveryMethodFactory>(MockBehavior.Strict);

		mockDeliveryMethodFactory.Setup(m => m.Create(medium).IsValidDestination(destination)).Returns(true);

		var webApplicationFactory = BaseWebApplicationFactory.Create()
			.WithDatabase(fixture)
			.WithTime(baseTime, out mockTimeProvider)
			.WithWebHostBuilder(web =>
			{
				web.ConfigureTestServices(svc =>
				{
					svc.AddSingleton(mockDeliveryMethodFactory.Object);
				});
			})
			.AddAppConfiguration(fixture, applicationId, new()
			{
				ConfigurationId = configurationId,
				ExpirationWindow = expirationWindow,
				IsSliding = isSliding,
				MaxAttemptCount = attemptCount,
			});

		using var db = new OtpDbContext(fixture.ContextOptions);
		if (otp != null)
		{
			db.DeliveredPasswords.Add(new()
			{
				ApplicationId = applicationId,
				MediumCode = medium.ToMediumCode(),
				DeliveryTarget = destination,
				ExpirationTime = baseTime.Add(expirationWindow),
				RemainingCount = attemptCount,
				PasswordHash = PasswordHashing.GeneratePasswordHash(otp),
			});
			await db.SaveChangesAsync();
		}

		var client = webApplicationFactory.CreateApiClient(apiKey: apiKey);
		return client;
	}

	[Fact]
	public async Task Verify_a_correct_otp_given_valid_configurations()
	{
		// Arrange
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var expirationWindow = TimeSpan.FromHours(1);
		var attemptCount = 3;
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: otp,
			expirationWindow: expirationWindow,
			attemptCount: attemptCount
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
	public async Task Decrements_the_number_of_uses_for_an_invalid_otp()
	{
		// Arrange
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var expirationWindow = TimeSpan.FromHours(1);
		var attemptCount = 3;
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: otp,
			expirationWindow: expirationWindow,
			attemptCount: attemptCount
		);

		// Act
		var response = await client.VerifyOtp(new(medium, destination, otp + "bad"));

		// Assert
		var result = await response.Response.Content.ReadAsStringAsync();
		Assert.IsType<Operations.VerifyOtpReturnType.Conflict>(response);

		using var db = new OtpDbContext(fixture.ContextOptions);
		var deliveredRecord = db.DeliveredPasswords.SingleOrDefault(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == OtpDbContextExtensions.ToMediumCode((Controllers.Medium)medium)
			&& x.DeliveryTarget == destination
		);
		Assert.NotNull(deliveredRecord);
		Assert.Equal(attemptCount - 1, deliveredRecord.RemainingCount);
	}

	[Fact]
	public async Task Decrements_the_number_of_uses_and_updates_expiration_for_an_invalid_otp()
	{
		// Arrange
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var expirationWindow = TimeSpan.FromHours(1);
		var attemptCount = 3;
		var nextTime = baseTime + TimeSpan.FromHours(0.5);
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: otp,
			expirationWindow: expirationWindow,
			attemptCount: attemptCount,
			isSliding: true
		);
		mockTimeProvider!.Setup(tp => tp.GetUtcNow()).Returns(nextTime);

		// Act
		var response = await client.VerifyOtp(new(medium, destination, otp + "bad"));

		// Assert
		var result = await response.Response.Content.ReadAsStringAsync();
		Assert.IsType<Operations.VerifyOtpReturnType.Conflict>(response);

		using var db = new OtpDbContext(fixture.ContextOptions);
		var deliveredRecord = db.DeliveredPasswords.SingleOrDefault(
			x => x.ApplicationId == applicationId
			&& x.MediumCode == OtpDbContextExtensions.ToMediumCode((Controllers.Medium)medium)
			&& x.DeliveryTarget == destination
		);
		Assert.NotNull(deliveredRecord);
		Assert.Equal(attemptCount - 1, deliveredRecord.RemainingCount);
		Assert.Equal(nextTime + expirationWindow, deliveredRecord.ExpirationTime);
	}

	[Fact]
	public async Task Removes_an_otp_with_no_more_sends_available()
	{
		// Arrange
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var expirationWindow = TimeSpan.FromHours(1);
		var attemptCount = 1;
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: otp,
			expirationWindow: expirationWindow,
			attemptCount: attemptCount
		);

		// Act
		var response = await client.VerifyOtp(new(medium, destination, otp + "bad"));

		// Assert
		var result = await response.Response.Content.ReadAsStringAsync();
		Assert.IsType<Operations.VerifyOtpReturnType.Conflict>(response);

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
		var medium = Medium.Sms;
		var destination = "+15554545544";
		var otp = "123456";
		var expirationWindow = TimeSpan.FromHours(1);
		var attemptCount = 3;
		var client = await ConfigureApplication(
			apiMedium: medium,
			destination: destination,
			otp: null,
			expirationWindow: expirationWindow,
			attemptCount: attemptCount
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
