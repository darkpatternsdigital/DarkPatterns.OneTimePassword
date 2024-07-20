namespace DarkPatterns.OneTimePassword.Auth;

public static class ServiceRegistration
{
	internal static void RegisterAuth(this IServiceCollection services)
	{
		/* Authentication and Authorization in ASP.NET is built into a series of
		 * separated concerns.
		 *
		 * Authentication is built with a series of schemes, each of which may have
		 * implemented up to three types of scheme implementations.
		 * - "Authenticate Schemes" load information about the user from the
		 *   HttpContext to create the ClaimsPrincipal.
		 * - "Sign In Schemes" save information about the user (such as to the
		 *   HttpResponse).
		 * - "Challenge Schemes" are used to request information from the user to
		 *   authenticate themselves, such as redirecting to an OAuth page.
		 *
		 * Each scheme must implement at least one of the above types. They get
		 * composed to make a complete solution.
		 * - .NET's default cookie scheme only supports Authenticate and Sign In; it
		 *   reads or writes cookies. There is no way to "challenge" a user to sign in
		 *   via a Cookie scheme.
		 * - .NET's OAuth schemes only support Challenge; they save the received user
		 *   claims via the configured Sign In scheme.
		 * - Something like HTTP Basic Authentication or an API Key rule would be
		 *   implemented via only an Authenticate scheme; the server does not send
		 *   credentials to the end user for these rules.
		 *
		 * Authorization uses "policies" to authorize requests. These load credentials
		 * from one or more "Authenticate Schemes", then applies a set of
		 * requirements to the loaded ClaimsPrincipal. If the claims do not pass the
		 * requirements, either the `OnRedirectToAccessDenied` or `OnRedirectToLogin`
		 * is called, depending on whether the Authentication Scheme was able to load
		 * any credentials.
		 * */

		services.AddAuthorization(options =>
		{
			options.AddPolicy("LicensedApp", builder =>
			{
				// TODO: Verify x-api-key Header
				// This "always allowed" requirement satisfies ASP.Net's
				// requirement of having at least one assertion for a policy
				builder.RequireAssertion(context => true);
			});
		});

	}

}
