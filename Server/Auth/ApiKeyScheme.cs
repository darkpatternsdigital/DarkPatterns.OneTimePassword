

using System.Security.Claims;
using System.Text.Encodings.Web;

using DarkPatterns.OneTimePassword.Persistence;

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DarkPatterns.OneTimePassword.Auth;

public class ApiKeyScheme : AuthenticationHandler<ApiKeyOptions>
{
	public const string ApiKeyHeaderName = "x-api-key";

	public ApiKeyScheme(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
	{
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var headers))
			return AuthenticateResult.Fail("x-api-key header must be provided");
		var parsed = headers is [string apiKey] ? ApiKeyTools.ParseApiKey(apiKey) : null;
		if (parsed == null)
			return AuthenticateResult.Fail("Only one x-api-key header may be provided");
		var (appId, configId) = parsed;

		var dbContext = Context.RequestServices.GetRequiredService<OtpDbContext>();
		var hasApp = await dbContext.ConfiguredApplications.FirstOrDefaultAsync(cfgApp => cfgApp.ApplicationId == appId && cfgApp.ConfigurationId == configId);
		if (hasApp == null)
			return AuthenticateResult.Fail("Invalid API Key");

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
