using DarkPatterns.OneTimePassword.Environment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterEnvironment(
	isProduction: builder.Environment.IsProduction(),
	environmentName: builder.Environment.EnvironmentName,
	buildConfig: builder.Configuration.GetSection("build"),
	dataProtectionConfig: builder.Configuration.GetSection("DataProtection")
);

var app = builder.Build();

app.UseHealthChecks("/health");

app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.Run();
