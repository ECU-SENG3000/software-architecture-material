# .NET Aspire Cheatsheet for Beginners

## The short version

**Aspire helps you develop and observe an application made of multiple services.** You describe the parts of your application and how they connect in an **AppHost** project. Aspire starts them together and gives you a dashboard for their status, endpoints, logs, traces, and metrics.

Aspire is not your web framework, database, or production cloud. Your services still use their normal frameworks and libraries; Aspire coordinates them during development and provides tools and integrations around them.

```text
AppHost (describes and starts the application)
  ├── Web frontend ──references──> API
  └── API ──references──> Redis
         │
         └── Aspire dashboard: status, endpoints, logs, traces, metrics
```

## 1. The important terms

| Term | What it means |
| --- | --- |
| **AppHost** | The orchestration project. It declares the application resources and their relationships. It usually contains no business logic. |
| **Resource** | Anything Aspire knows how to run or track: a project, executable, container, database, cache, or other service. |
| **Project resource** | An application project added to the AppHost, typically with `AddProject<T>()`. |
| **Container resource** | A containerized dependency such as Redis, added through a hosting integration. A container runtime such as Docker Desktop may be required. |
| **Reference** | A declared relationship between resources. `WithReference(...)` supplies connection or endpoint information to the consuming resource. |
| **Service discovery** | Resolving a referenced service by its logical name rather than hard-coding a machine-specific URL. |
| **ServiceDefaults** | A shared project in many .NET Aspire templates. Its `AddServiceDefaults()` extension configures common concerns such as telemetry, health checks, service discovery, and resilience. |
| **Dashboard** | The local web UI for seeing resource state and endpoints, and exploring logs, traces, and metrics. |
| **Integration** | A package that helps model a dependency in the AppHost (hosting integration), use it from an app (client integration), or both. |

**Useful distinction:** the AppHost describes the system; the individual services implement the system.

## 2. Create and run a starter app

Install the .NET SDK and Aspire CLI first. This repository's [getting started guide](./getting-started.md) covers the course workstation setup.

Create the C# starter solution:

```bash
aspire new aspire-starter -n AspireApp -o AspireApp
cd AspireApp
aspire run
```

The starter template includes an AppHost, an API service, a web frontend, and a ServiceDefaults project. `aspire run` finds the AppHost, builds and starts the application, and prints the dashboard URL. Open that URL and use the resource endpoints to try the running app. Stop the local application with **Ctrl+C**.

If you already have an AppHost project, run `aspire run` from its solution directory (or point the CLI at the AppHost as described in the [CLI reference](https://aspire.dev/reference/cli/commands/aspire-run/)).

## 3. Read the AppHost

A simplified C# AppHost might look like this:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MyApp_Api>("api");

builder.AddProject<Projects.MyApp_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
```

What each line is doing:

- `CreateBuilder(args)` creates the distributed-application builder.
- `AddProject<T>("api")` adds the API project as a named resource.
- `AddProject<T>("web")` adds the web project.
- `WithReference(api)` declares that the web project uses the API and provides the information needed for service discovery.
- `WaitFor(api)` expresses a startup dependency: wait for the API to be ready before starting the web project. It does not replace retries or error handling in your application.
- `Build().Run()` starts the AppHost.

Names such as `"api"` are resource names used by Aspire and service discovery; they are not necessarily the project or assembly names.

The exact project types and extra health-check or endpoint configuration vary by template. Treat the generated AppHost as the source of truth for your solution.

## 4. Add a dependency (example: Redis)

Most integrations follow the same pattern:

1. Add a **hosting integration** to the AppHost.
2. Declare the dependency as a resource.
3. Add a reference from each application that needs it.
4. Add the appropriate **client integration** to the consuming application and use it in code.

For example, the AppHost side for Redis typically looks like:

```csharp
var cache = builder.AddRedis("cache");

builder.AddProject<Projects.MyApp_Api>("api")
    .WithReference(cache);
```

The exact package and API depend on the integration. Follow its documentation or use the Aspire CLI's integration workflow (for example, `aspire add redis`) to add the package. The AppHost hosting package models and starts/configures the resource; it does **not** by itself add the Redis client library or application code needed to use Redis.

When a project references a database or cache, Aspire can provide its connection information to that project. Keep the actual client usage in the service that owns the dependency; do not put business logic in the AppHost.

## 5. ServiceDefaults in application projects

In a template-generated ASP.NET Core service, you will commonly see:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();
app.Run();
```

`AddServiceDefaults()` is an extension method from the template's ServiceDefaults project. It configures shared development and production-friendly service behavior, commonly including OpenTelemetry instrumentation/export, health checks, service discovery, and HTTP resilience. `MapDefaultEndpoints()` exposes the template's default health endpoints (often `/health` and `/alive`).

These helpers are ordinary .NET configuration code, not magic that makes every app Aspire-aware. Check the generated ServiceDefaults source and tailor it to your application's needs.

## 6. Environments and service discovery

An **environment** is the runtime context for an application, commonly named `Development`, `Staging`, or `Production`. The name can be customized. In .NET, the environment name controls environment-specific behavior such as loading `appsettings.{Environment}.json` and checking `app.Environment.IsDevelopment()`. It is not itself a separate network or a way for services to find each other.

The environment name is commonly set through `DOTNET_ENVIRONMENT` for the .NET Generic Host or `ASPNETCORE_ENVIRONMENT` for ASP.NET Core. `WebApplication` honors `DOTNET_ENVIRONMENT` ahead of `ASPNETCORE_ENVIRONMENT` if both are set, so prefer setting one deliberately. In Aspire, the AppHost describes the resources and their references. Aspire starts each resource with its own process environment and supplies connection details for declared references. Local endpoint addresses and ports can change, so application code should use service discovery or injected connection details instead of assuming a fixed `localhost` URL. A deployed environment may use different addresses and settings without requiring different application code.

For example, when the AppHost references the API from the web project:

```csharp
var api = builder.AddProject<Projects.MyApp_Api>("api");

builder.AddProject<Projects.MyApp_Web>("web")
    .WithReference(api);
```

Aspire injects environment variables into the **web** resource for the referenced API. Common variable shapes are:

| Purpose | Example | How to use it |
| --- | --- | --- |
| .NET service-discovery endpoint | `services__api__http__0` | Read by .NET service discovery from configuration; the value is the resolved endpoint URL. |
| Direct endpoint URL | `API_HTTP` | Useful to a non-.NET consumer that reads environment variables directly. Endpoint and resource names are encoded and uppercased. |
| Connection string | `ConnectionStrings__cache` | Read as `builder.Configuration.GetConnectionString("cache")` or by a compatible client integration. |

The exact variables depend on the resource and endpoint names. Use the Aspire dashboard's resource details to inspect the values supplied to a running resource. A reference only gives a consumer configuration for resources it explicitly depends on; add `.WithReference(...)` for each dependency.

For a .NET HTTP client, ServiceDefaults configures service discovery in the usual Aspire template. Use the logical service name rather than a concrete address:

```csharp
builder.Services.AddHttpClient("api", client =>
    client.BaseAddress = new Uri("https+http://api"));
```

`https+http://api` asks the resolver to use an HTTPS endpoint when available and otherwise an HTTP endpoint. The service-discovery handler resolves `api` using the endpoint information Aspire injected into configuration. For a service with a named endpoint, use the `https+http://_endpointName.api` form. If using an older client setup or a project without ServiceDefaults, verify that its service-discovery handler is registered.

Use `.WithEnvironment("NAME", "value")` in the AppHost when a resource needs an explicitly set environment variable. Keep environment-specific values outside business logic, and do not put production secrets in source code or ordinary checked-in settings files.

## 7. Where .NET applications get configuration

In a typical ASP.NET Core app, `WebApplication.CreateBuilder(args)` combines configuration providers into `builder.Configuration` (`IConfiguration`). With the standard defaults, providers are loaded in this order; when the same key occurs more than once, the later provider wins:

1. `appsettings.json`
2. `appsettings.{Environment}.json` (for example, `appsettings.Development.json`)
3. User Secrets, when configured and running in `Development`
4. Environment variables
5. Command-line arguments

For example, the hierarchical key `Catalog:BaseUrl` can be set in JSON as `{"Catalog":{"BaseUrl":"https://example"}}` or in an environment variable as `Catalog__BaseUrl`. The double underscore (`__`) is the portable environment-variable separator for configuration keys. Environment variables and command-line arguments can override file and User Secrets values. Providers added later in application code can override the defaults too.

Other sources can be added when needed:

| Source | Typical use |
| --- | --- |
| JSON, INI, or XML files | Structured settings; JSON is the common default. Additional files can be added with providers such as `AddJsonFile`, `AddIniFile`, or `AddXmlFile`. |
| .NET User Secrets | Local development secrets stored outside the project directory. They are not encrypted and are not a production secret store. |
| Environment variables | Shell, container, Aspire, orchestrator, or hosting-platform settings. Docker, Kubernetes, and cloud app settings commonly reach the process this way. |
| Command-line arguments | Per-run overrides, especially for local tooling and deployments. |
| In-memory values | Tests, generated defaults, or values added by application code. |
| External providers | Azure App Configuration, Azure Key Vault, a database, or another service through a provider package or custom `IConfigurationProvider`. |
| Key-per-file | Container and orchestrator setups that mount each setting or secret as a separate file. |

`launchSettings.json` is used by development tools to launch a process with a profile; it is not itself a configuration provider read by the application. Its environment variables and arguments become inputs to the process. Likewise, an `.env` file is not automatically loaded by .NET unless a tool or package loads it.

Use `IConfiguration` for individual values and bind related settings to typed options when appropriate. Store only non-secret defaults in checked-in settings files; use User Secrets for local development and an appropriate secret provider or protected platform configuration for deployed applications.

## 8. Explore the dashboard

The dashboard is the first place to look when the composed application does not behave as expected:

- **Resources:** see whether each service or dependency is starting, running, finished, or failing; open available endpoints.
- **Console logs:** read output written by individual resources.
- **Structured logs:** filter application log events and inspect their properties.
- **Traces:** follow a request across service boundaries and find where time was spent or an error occurred.
- **Metrics:** inspect measurements emitted by instrumented services.

Logs, traces, and metrics are forms of **telemetry**. They are most useful when the services are instrumented and send their telemetry to the Aspire dashboard. Aspire templates and ServiceDefaults configure much of this for common .NET scenarios.

The dashboard login URL printed by the CLI can contain an access token. Treat it as sensitive; do not post it publicly.

## 9. Common commands

| Command | Use |
| --- | --- |
| `aspire --version` | Check which Aspire CLI version is installed. |
| `aspire new aspire-starter -n AspireApp -o AspireApp` | Create a C# starter application. |
| `aspire run` | Build and run the AppHost and its resources locally. |
| `aspire add <integration>` | Add a supported integration to an AppHost. Check the integration's docs for its exact name and setup. |
| `aspire list` | Inspect CLI-provided project or integration choices when supported by your installed CLI version. |

Aspire CLI commands evolve; consult the [current CLI reference](https://aspire.dev/reference/cli/) if a command or option differs in your installed version.

## 10. Common beginner problems

| Symptom | Things to check |
| --- | --- |
| `aspire run` cannot find an AppHost | Run it from the solution/repository directory containing the AppHost, or use the CLI's AppHost selection option. |
| A container resource will not start | Check that Docker Desktop or another supported container runtime is installed and running; read the resource's console logs. |
| One service cannot reach another | Confirm the consumer has `.WithReference(theResource)` and that the app uses Aspire service discovery rather than a stale hard-coded local URL. |
| A dependent service starts too early | Add an appropriate `WaitFor(...)` relationship when startup ordering/readiness matters, and still handle transient failures in the application. |
| The dashboard has no traces or metrics | Check that the service registers instrumentation (for example, ServiceDefaults), emits telemetry, and is running under the AppHost. |
| A dependency has no data after restarting | Check whether the resource uses persistent storage. Container lifetime and data persistence are separate concerns. |
| A port or endpoint is different from yesterday | Avoid assuming fixed local ports. Use the endpoint shown in the dashboard or service discovery. |
| HTTPS reports a local certificate warning | Follow the CLI output and the [Aspire certificate guidance](https://aspire.dev/get-started/installation/). Do not disable certificate validation as a workaround. |

## 11. Keep these rules in mind

- Aspire is especially useful for **local development and orchestration** of distributed applications. It can also support publishing workflows, but deployment still needs an appropriate target and configuration.
- Keep business logic in application projects, not in the AppHost.
- Add references explicitly. If an app depends on a resource, model that relationship in the AppHost.
- Prefer service discovery and injected connection details to hard-coded local addresses and credentials.
- A container is not automatically persistent. Configure storage deliberately when data must survive container replacement.
- `WaitFor(...)` coordinates startup readiness; it is not a substitute for application-level resilience.
- The dashboard helps you observe the application, but your app still needs correct logging and instrumentation.
- Aspire does not require every part of the application to be .NET; the AppHost can orchestrate supported non-.NET applications too.

## Official documentation

- [Build your first Aspire app](https://aspire.dev/get-started/build-your-first-aspire-app/)
- [AppHost overview](https://aspire.dev/get-started/app-host/)
- [Aspire CLI overview](https://aspire.dev/get-started/aspire-cli-overview/)
- [Service discovery](https://aspire.dev/fundamentals/service-discovery/)
- [Aspire environment variables](https://aspire.dev/fundamentals/environment-variables/)
- [C# Service Defaults](https://aspire.dev/fundamentals/service-defaults/)
- [.NET configuration in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0)
- [.NET configuration providers](https://learn.microsoft.com/dotnet/core/extensions/configuration-providers)
- [Safe storage of app secrets in development](https://learn.microsoft.com/aspnet/core/security/app-secrets?view=aspnetcore-10.0)
- [Aspire dashboard overview](https://aspire.dev/dashboard/overview/)
- [Aspire integrations](https://aspire.dev/integrations/)