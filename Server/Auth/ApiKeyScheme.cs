

using System.Security.Claims;
using System.Text.Encodings.Web;

using DarkPatterns.OneTimePassword.Persistence;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DarkPatterns.OneTimePassword.Auth;

public class ApiKeyScheme : AuthenticationHandler<ApiKeyOptions>
{
	private const string apiKeyHeaderName = "x-api-key";

	public ApiKeyScheme(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
	{
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Context.Request.Headers.TryGetValue(apiKeyHeaderName, out var apiKey))
			return AuthenticateResult.Fail("x-api-key header must beprovided");

		var dbContext = Context.RequestServices.GetRequiredService<OtpDbContext>();
		// TODO - verify API Key exists in dbContext
		await Task.Yield();

		var ticket = new AuthenticationTicket(
			new ClaimsPrincipal(
				new ClaimsIdentity([
					new Claim(ApiKeyClaimTypes.ApplicationId, Guid.Empty.ToString()),
					new Claim(ApiKeyClaimTypes.ConfigurationId, Guid.Empty.ToString()),
				], this.Scheme.Name)
			),
			this.Scheme.Name
		);
		return AuthenticateResult.Success(ticket);
	}
}