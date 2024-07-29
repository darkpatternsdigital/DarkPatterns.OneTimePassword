# DarkPatterns.OneTimePassword

A sample microservice leveraging [Dark Patterns Digital's OpenAPI code generators][dpd-openapi-generators]

## Structure

- /eng - Contains repo-level structures, including:
  - .NET auto-code-formatting
  - pnpm-msbuild integration
  - API Key generation scripts
  - Docker tools for running postgres locally
- /schemas - OpenAPI Schema for the One Time Password API
- /Server - .NET Server implementation of the OTP microservice
- /Server.Tests - .NET tests for the OTP Server

## Development

Development is orchestrated via msbuild.

Prerequisites:
- [.NET 8.0.x SDK][dotnet-8]

To run locally, use one of the following options:

- Using Visual Studio:
  1. Open `./DarkPatterns.OneTimePassword.sln`.
  2. Set up local configuration (see below)
  3. Debug or run the `Server` project.

- Using the `dotnet` CLI:
  1. Set up local configuration (see below)
  2. Run the following commands in your terminal:
     ```sh
     cd Server
     dotnet run
     ```

[dotnet-8]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
[dpd-openapi-generators]: https://github.com/darkpatternsdigital/openapi-generators
