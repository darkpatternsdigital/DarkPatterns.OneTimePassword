using DarkPatterns.OneTimePassword.Auth;
using DarkPatterns.OneTimePassword.Delivery;
using DarkPatterns.OneTimePassword.Environment;
using DarkPatterns.OneTimePassword.Logic;
using DarkPatterns.OneTimePassword.Persistence;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterEnvironment(
	isProduction: builder.Environment.IsProduction(),
	environmentName: builder.Environment.EnvironmentName,
	buildConfig: builder.Configuration.GetSection("build"),
	dataProtectionConfig: builder.Configuration.GetSection("DataProtection")
);
builder.Services.RegisterAuth();
builder.Services.RegisterDelivery();
builder.Services.RegisterLogic();
if (builder.Configuration["Postgres:ConnectionString"] is string psqlConnectionString)
	builder.Services.RegisterNpgsqlPersistence(psqlConnectionString);

var app = builder.Build();

if (!builder.Environment.IsProduction())
	app.UseDeveloperExceptionPage();

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

if (args.Contains("--ef-migrate"))
{
	await StartupUtils.RunMigrations(app.Services);
	return;
}
if (args.Contains("--ef-verify"))
{
	await StartupUtils.CheckMigrations(app.Services);
}

app.Run();
