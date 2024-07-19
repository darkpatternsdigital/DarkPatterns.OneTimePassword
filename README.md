# Modern .NET Server + React with Vite

A web application starter with a Backend for Frontend built with .NET, with an
OpenAPI file to define the API layer, along with a UI built with React and Vite.

## Development

Development is orchestrated via msbuild.

Prerequisites:
- [.NET 8.0.x SDK][dotnet-8]
- [.NET 7.0.x Runtime][codegen-dotnet-version] (for the [Principle Studios
  OpenAPI code generators][ps-openapi-codegen])
- [pnpm][pnpm-setup]

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

- Within the `ui` folder:
  1. Set up local configuration (see below)
  2. Run the following commands in your terminal:
     ```sh
     cd ui
     pnpm start
     ```

### Local Configuration (Optional)

The project may run locally without any local configuration with limited
functionality. To configure the application for local development:

1. Create a `.env` file in the root of the repository
2. Set up desired environment variables.

### Other components

- **Jaeger**

    If you have `docker-compose` installed and your container runtime started at
    the time of building, Jaeger will be automatically started at
    http://localhost:16686/ for collecting local OpenTelementry data.

## Hosting

The application can be built with a `Dockerfile` and deployed as an all-in-one
container.

[dotnet-8]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
[codegen-dotnet-version]: https://dotnet.microsoft.com/en-us/download/dotnet/7.0
[ps-openapi-codegen]: https://github.com/PrincipleStudios/principle-studios-openapi-generators
[pnpm-setup]: https://pnpm.io/cli/install
