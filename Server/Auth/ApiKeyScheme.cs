

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
		if (!Context.Request.Headers.TryGetValue(apiKeyHeaderName, out var headers))
			return AuthenticateResult.Fail("x-api-key header must be provided");
		var parsed = headers is [string apiKey] ? ApiKeyTools.ParseApiKey(apiKey) : null;
		if (parsed == null)
			return AuthenticateResult.Fail("Only one x-api-key header may be provided");
		var (appId, configId) = parsed;

		var dbContext = Context.RequestServices.GetRequiredService<OtpDbContext>();
		// TODO - verify API Key exists in dbContext
		await Task.Yield();

		var ticket = new AuthenticationTicket(
			new ClaimsPrincipal(
				new ClaimsIdentity([
					new Claim(ApiKeyClaimTypes.ApplicationId, appId.ToString()),
					new Claim(ApiKeyClaimTypes.ConfigurationId, configId.ToString()),
				], this.Scheme.Name)
			),
			this.Scheme.Name
		);
		return AuthenticateResult.Success(ticket);
	}
}
