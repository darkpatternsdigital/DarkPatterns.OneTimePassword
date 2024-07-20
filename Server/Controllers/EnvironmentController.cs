
using DarkPatterns.OneTimePassword.Environment;

using Microsoft.Extensions.Options;

namespace DarkPatterns.OneTimePassword.Controllers;

public class EnvironmentController : EnvControllerBase
{
	private readonly BuildOptions buildOptions;

	public EnvironmentController(IOptions<BuildOptions> options)
	{
		buildOptions = options.Value;
	}

	protected override Task<GetInfoActionResult> GetInfo()
	{
		return Task.FromResult(
			GetInfoActionResult.Ok(new(buildOptions.GitHash, buildOptions.Tag))
		);
	}
}
