# 🚀 .NET C# Dependency Injection Cheatsheet

## 🎯 Concise Takeaway
Dependency Injection (DI) in modern .NET is built on **`Microsoft.Extensions.DependencyInjection`**. It separates **registration** (configuring `IServiceCollection` at startup) from **resolution** (retrieving instances via `IServiceProvider` at runtime).

- **Three Core Lifetimes:** `Transient` (new each request), `Scoped` (one per HTTP request/scope), and `Singleton` (one per application).
- **Keyed Services (.NET 8+):** Register and resolve distinct implementations of the same interface using named or typed keys via `AddKeyed*` and `[FromKeyedServices]`.
- **Primary Resolution Pattern:** Constructor injection (including C# 12 Primary Constructors). Avoid manual `IServiceProvider` calls (Service Locator anti-pattern) except in hosted worker loops or factory abstractions.

---

# 📑 Table of Contents
1. [Core Mental Model](#1-core-mental-model)
2. [Service Lifetimes Comparison](#2-service-lifetimes-comparison)
3. [Configuring Regular Services (Registration)](#3-configuring-regular-services-registration)
4. [Configuring Keyed Services (.NET 8+)](#4-configuring-keyed-services-net-8)
5. [Consuming Dependency Injected Services](#5-consuming-dependency-injected-services)
   - [Constructor Injection (Traditional & Primary Constructors)](#a-constructor-injection-preferred)
   - [Consuming Keyed Services](#b-consuming-keyed-services)
   - [Minimal APIs](#c-minimal-apis-parameter-binding)
   - [Controllers (Action Injection)](#d-aspnet-core-mvc--api-controllers)
   - [Blazor Components](#e-blazor-components)
   - [Background Services & Manual Scope Creation](#f-background-services-and-custom-scopes)
   - [Direct IServiceProvider Resolution](#g-direct-iserviceprovider-runtime-resolution)
   - [Resolving Multiple Implementations](#h-resolving-multiple-implementations-ienumerablet)
6. [Disposal & Lifecycle Rules](#6-disposal--lifecycle-rules)
7. [Critical Anti-Patterns & Pitfalls](#7-critical-anti-patterns--pitfalls)
8. [Quick Reference Cheatsheet Table](#8-quick-reference-cheatsheet-table)

---

# 1. Core Mental Model

```text
┌─────────────────────────────────┐       BuildServiceProvider()      ┌─────────────────────────────────┐
│       IServiceCollection        │ ─────────────────────────────────► │        IServiceProvider         │
│  (Service Descriptors & Config) │                                    │  (Instance Resolution Engine)   │
└─────────────────────────────────┘                                    └─────────────────────────────────┘
                 ▲                                                                      │
                 │ Registered at startup                                                │ Resolves dependencies
     builder.Services.Add*()                                                            ▼
                                                                             Constructor / Action / API
```

- **`ServiceDescriptor`**: Describes a service type, its implementation type/factory/instance, lifetime, and optional service key.
- **`IServiceCollection`**: A list of `ServiceDescriptor` items configured during app bootstrapping.
- **`IServiceProvider`**: The compiled IoC container that instantiates dependencies and manages their lifecycles.

---

# 2. Service Lifetimes Comparison

| Lifetime | Method | Instances Created | Common Use Cases | Thread Safety Required? |
| :--- | :--- | :--- | :--- | :--- |
| **Transient** | `AddTransient` | Every single time requested | Lightweight, stateless utilities, validators, formatters | ❌ No (isolated per call) |
| **Scoped** | `AddScoped` | Once per scope (e.g., HTTP request) | EF Core `DbContext`, per-request state, unit of work, current user context | ❌ No (within a single thread/request) |
| **Singleton** | `AddSingleton` | Exactly once for app lifetime | In-memory cache, message bus client, configuration, metrics aggregator |  **Yes** (shared across threads) |

### ⚠️ Scope Validation in Development
Always keep scope validation enabled to detect **Captive Dependencies** early:

```csharp
// Default in WebApplication.CreateBuilder(args):
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = context.HostingEnvironment.IsDevelopment();
});
```

---

# 3. Configuring Regular Services (Registration)

### A. Interface with Implementation Class
The standard, most common pattern:

```csharp
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

### B. Concrete Class (Self-Registration)
When no abstraction is required:

```csharp
builder.Services.AddTransient<OrderCalculator>();
builder.Services.AddScoped<OrderProcessingWorkflow>();
builder.Services.AddSingleton<AppMetrics>();
```

### C. Factory Delegate (Dynamic Initialization)
Use when instantiation requires logic or dependencies from the container:

```csharp
builder.Services.AddScoped<IPaymentGateway>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var logger = serviceProvider.GetRequiredService<ILogger<StripePaymentGateway>>();
    var apiKey = config["Stripe:ApiKey"] ?? throw new InvalidOperationException("API key missing");
    
    return new StripePaymentGateway(apiKey, logger);
});
```

### D. Existing Instance (Singleton Only)
Register an already-instantiated object:

```csharp
var externalClient = new ThirdPartyApiClient("token_123");
builder.Services.AddSingleton<IThirdPartyApiClient>(externalClient);
// ⚠️ Note: The DI container will NOT dispose instances registered this way!
```

### E. Open Generics
Register generic interfaces mapped to open generic types:

```csharp
// Resolves IRepository<Order> as Repository<Order>, IRepository<User> as Repository<User>
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

### F. Multiple Implementations for the Same Interface
Registering multiple types preserves all of them in order:

```csharp
builder.Services.AddTransient<INotificationChannel, EmailNotificationChannel>();
builder.Services.AddTransient<INotificationChannel, SmsNotificationChannel>();
builder.Services.AddTransient<INotificationChannel, PushNotificationChannel>();
```

> **Note:** Calling `GetRequiredService<INotificationChannel>()` returns the **last** registered implementation (`PushNotificationChannel`). To retrieve all of them, inject `IEnumerable<INotificationChannel>`.

### G. Idempotent Registration (`TryAdd` & `TryAddEnumerable`)
Avoid duplicate registrations across extension methods:

```csharp
using Microsoft.Extensions.DependencyInjection.Extensions;

// Registers only if IOrderService is not yet registered:
builder.Services.TryAddScoped<IOrderService, OrderService>();

// Registers only if OrderService is not yet registered for IOrderService:
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<INotificationChannel, EmailNotificationChannel>());
```

### H. Options Pattern Registration
Binding configuration sections to strongly-typed classes:

```csharp
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

// Or with fluent validation:
builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### I. Modular Clean Architecture Registration Pattern
Organize registrations into focused extension methods per project/layer:

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IOrderRepository, SqlOrderRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}

// In Program.cs:
builder.Services.AddInfrastructureServices(builder.Configuration);
```

---

# 4. Configuring Keyed Services (.NET 8+)

Keyed services let you register multiple implementations of the same service contract identified by a **key** (`string`, `enum`, or any `object`).

```csharp
public interface INotificationSender
{
    Task SendAsync(string recipient, string message);
}

public class EmailNotificationSender : INotificationSender { /* ... */ }
public class SmsNotificationSender : INotificationSender { /* ... */ }
public class WhatsAppNotificationSender : INotificationSender { /* ... */ }
public class FallbackNotificationSender : INotificationSender { /* ... */ }
```

### A. Registering Keyed Services by Lifetime

```csharp
// String keys
builder.Services.AddKeyedTransient<INotificationSender, EmailNotificationSender>("email");
builder.Services.AddKeyedScoped<INotificationSender, SmsNotificationSender>("sms");
builder.Services.AddKeyedSingleton<INotificationSender, WhatsAppNotificationSender>("whatsapp");

// Enum keys (strongly typed)
public enum StorageType { AzureBlob, AmazonS3, LocalDisk }

builder.Services.AddKeyedScoped<IFileStorage, AzureBlobStorage>(StorageType.AzureBlob);
builder.Services.AddKeyedScoped<IFileStorage, S3Storage>(StorageType.AmazonS3);
builder.Services.AddKeyedScoped<IFileStorage, LocalDiskStorage>(StorageType.LocalDisk);
```

### B. Keyed Factory Registration
Access both the `IServiceProvider` and the matched `serviceKey` in the factory:

```csharp
builder.Services.AddKeyedScoped<IPaymentProcessor>("paypal", (sp, key) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<PayPalProcessor>>();
    return new PayPalProcessor(config["PayPal:Secret"]!, logger);
});
```

### C. Wildcard Fallback Registration (`KeyedService.AnyKey`)
Use `KeyedService.AnyKey` to register a fallback implementation for any unrecognized key:

```csharp
builder.Services.AddKeyedTransient<INotificationSender, FallbackNotificationSender>(KeyedService.AnyKey);
```

### D. Idempotent Keyed Registrations
```csharp
using Microsoft.Extensions.DependencyInjection.Extensions;

builder.Services.TryAddKeyedTransient<INotificationSender, EmailNotificationSender>("email");
```

---

# 5. Consuming Dependency Injected Services

## A. Constructor Injection (Preferred)

Constructor injection guarantees all dependencies are supplied when an object is created.

### Traditional Constructor (C# 11 and earlier)
```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repository,
        IEmailSender emailSender,
        ILogger<OrderService> logger)
    {
        _repository = repository;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task ProcessOrderAsync(Order order)
    {
        _logger.LogInformation("Processing order {Id}", order.Id);
        await _repository.SaveAsync(order);
        await _emailSender.SendEmailAsync(order.CustomerEmail, "Order Confirmed");
    }
}
```

### C# 12+ Primary Constructors (Concise & Clean)
```csharp
public class OrderService(
    IOrderRepository repository,
    IEmailSender emailSender,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task ProcessOrderAsync(Order order)
    {
        logger.LogInformation("Processing order {Id}", order.Id);
        await repository.SaveAsync(order);
        await emailSender.SendEmailAsync(order.CustomerEmail, "Order Confirmed");
    }
}
```

---

## B. Consuming Keyed Services

Use the **`[FromKeyedServices(key)]`** attribute on constructor parameters.

### Traditional Constructor with Keyed Services
```csharp
public class NotificationDispatcher
{
    private readonly INotificationSender _emailSender;
    private readonly INotificationSender _smsSender;

    public NotificationDispatcher(
        [FromKeyedServices("email")] INotificationSender emailSender,
        [FromKeyedServices("sms")] INotificationSender smsSender)
    {
        _emailSender = emailSender;
        _smsSender = smsSender;
    }

    public async Task AlertUserAsync(string email, string phone, string message)
    {
        await _emailSender.SendAsync(email, message);
        await _smsSender.SendAsync(phone, message);
    }
}
```

### C# 12+ Primary Constructor with Keyed Services
```csharp
public class DocumentService(
    [FromKeyedServices(StorageType.AzureBlob)] IFileStorage cloudStorage,
    [FromKeyedServices(StorageType.LocalDisk)] IFileStorage localStorage,
    ILogger<DocumentService> logger)
{
    public async Task ArchiveDocumentAsync(string fileName, Stream content)
    {
        logger.LogInformation("Archiving {FileName} to cloud and local disk", fileName);
        await cloudStorage.UploadAsync(fileName, content);
        await localStorage.UploadAsync(fileName, content);
    }
}
```

---

## C. Minimal APIs Parameter Binding

ASP.NET Core Minimal APIs automatically resolve route parameters, bodies, and injected services.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Registrations
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddKeyedScoped<IPaymentGateway, StripePaymentGateway>("stripe");
builder.Services.AddKeyedScoped<IPaymentGateway, PayPalPaymentGateway>("paypal");

var app = builder.Build();

// Regular service parameter injection
app.MapPost("/orders", async (Order order, IOrderService orderService) =>
{
    await orderService.ProcessOrderAsync(order);
    return Results.Created($"/orders/{order.Id}", order);
});

// Keyed service injection via [FromKeyedServices]
app.MapPost("/checkout/stripe", async (
    CheckoutRequest request,
    [FromKeyedServices("stripe")] IPaymentGateway gateway) =>
{
    var result = await gateway.ChargeAsync(request.Amount);
    return Results.Ok(result);
});
```

---

## D. ASP.NET Core MVC / API Controllers

### Standard Constructor Injection (Recommended)
```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await orderService.GetByIdAsync(id);
        return order is not null ? Ok(order) : NotFound();
    }
}
```

### Action Method Injection (`[FromServices]` & `[FromKeyedServices]`)
Useful when a heavy dependency is only needed by a single action method:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport(
        ReportRequest request,
        [FromServices] IHeavyReportGenerator generator,
        [FromKeyedServices("excel")] IReportFormatter formatter)
    {
        var rawData = await generator.GenerateAsync(request);
        var formatted = formatter.Format(rawData);
        return File(formatted, "application/vnd.ms-excel");
    }
}
```

---

## E. Blazor Components

### Razor Syntax (`@inject`)
```razor
@page "/counter"
@inject ICounterService CounterService
@inject ILogger<Counter> Logger

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private void IncrementCount()
    {
        CounterService.Increment();
        Logger.LogInformation("Count incremented");
    }
}
```

### C# Code-Behind (`[Inject]` & Keyed `[Inject]`)
```csharp
public partial class NotificationsPanel
{
    [Inject]
    public required INotificationService GeneralNotifications { get; set; }

    [Inject(Key = "urgent")]
    public required INotificationService UrgentNotifications { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await UrgentNotifications.PollAsync();
    }
}
```

---

## F. Background Services and Custom Scopes

⚠️ **Critical Rule:** Singletons and long-running `BackgroundService` classes **must not** directly inject `Scoped` services (like EF Core `DbContext`). You must create an explicit scope using `IServiceScopeFactory`.

```csharp
public class OrderBatchBackgroundWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OrderBatchBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            // Create an explicit lifetime scope
            using (var scope = scopeFactory.CreateScope())
            {
                // Resolve scoped dependencies within this scope
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var orderProcessor = scope.ServiceProvider.GetRequiredService<IOrderProcessor>();
                
                // Also works for keyed services inside a scope
                var notifier = scope.ServiceProvider.GetRequiredKeyedService<INotificationSender>("email");

                var pendingOrders = await dbContext.Orders
                    .Where(o => o.Status == OrderStatus.Pending)
                    .ToListAsync(stoppingToken);

                foreach (var order in pendingOrders)
                {
                    await orderProcessor.ProcessAsync(order, stoppingToken);
                    await notifier.SendAsync(order.Email, "Order Processed");
                }
            } // Scope disposes here, releasing dbContext and scoped services

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

---

## G. Direct `IServiceProvider` Runtime Resolution

Use direct resolution sparingly (factories, middleware, dynamic dispatch).

```csharp
public class GatewayFactory(IServiceProvider serviceProvider)
{
    public IPaymentGateway GetGateway(string providerName)
    {
        // Regular resolution:
        // serviceProvider.GetService<T>() -> returns null if not registered
        // serviceProvider.GetRequiredService<T>() -> throws InvalidOperationException if not registered

        // Keyed resolution (.NET 8+):
        return serviceProvider.GetRequiredKeyedService<IPaymentGateway>(providerName);
    }

    public IPaymentGateway? TryGetGateway(string providerName)
    {
        return serviceProvider.GetKeyedService<IPaymentGateway>(providerName);
    }
}
```

---

## H. Resolving Multiple Implementations (`IEnumerable<T>`)

When multiple implementations of an interface are registered, inject `IEnumerable<T>` to receive all of them:

```csharp
public class HealthCheckAggregator(
    IEnumerable<IHealthCheckComponent> healthChecks,
    ILogger<HealthCheckAggregator> logger)
{
    public async Task<OverallStatus> CheckAllAsync()
    {
        foreach (var check in healthChecks)
        {
            var result = await check.CheckHealthAsync();
            logger.LogInformation("{CheckName}: {Status}", check.Name, result.IsHealthy);
        }

        return OverallStatus.Healthy;
    }
}
```

---

# 6. Disposal & Lifecycle Rules

The container automatically manages `IDisposable` and `IAsyncDisposable` instances that it creates:

1. **Transient instances** created within a scope are disposed when that **scope** is disposed.
2. **Scoped instances** are disposed when their containing **scope** is disposed (e.g., at the end of an HTTP request).
3. **Singleton instances** are disposed when the **root application container** is disposed (at application shutdown).
4. **Instances you instantiate yourself** (e.g., `AddSingleton(new MyService())`) are **NOT** disposed by the container. You own their lifecycle.

```csharp
public class ResourceHoldingService : IDisposable, IAsyncDisposable
{
    private readonly FileStream _fileStream = File.OpenRead("data.bin");

    public void Dispose()
    {
        _fileStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _fileStream.DisposeAsync();
    }
}
```

---

# 7. Critical Anti-Patterns & Pitfalls

### ❌ Anti-Pattern 1: The Captive Dependency
**What it is:** A service with a longer lifetime holds onto a service with a shorter lifetime (e.g., a `Singleton` taking a `Scoped` dependency like `DbContext`).
- **Impact:** Memory leaks, thread concurrency issues with `DbContext`, stale state across HTTP requests.
- **Fix:** Keep lifetimes equal or inject factory/scope (`IServiceScopeFactory`) inside the singleton.

```csharp
// 🚨 BUG: Singleton captures Scoped AppDbContext
public class OrderStatsCalculator(AppDbContext dbContext) // AppDbContext is Scoped!
{
}

//  CORRECT: Inject IServiceScopeFactory instead
public class OrderStatsCalculator(IServiceScopeFactory scopeFactory)
{
    public async Task<int> GetCountAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.Orders.CountAsync();
    }
}
```

---

### ❌ Anti-Pattern 2: The Service Locator
**What it is:** Passing `IServiceProvider` everywhere and calling `GetRequiredService<T>()` across business logic.
- **Impact:** Hides class dependencies, makes unit testing painful, couples business logic to the DI container.
- **Fix:** Explicitly declare needed dependencies in the constructor.

```csharp
// 🚨 BAD (Service Locator):
public class OrderProcessor(IServiceProvider provider)
{
    public void Process(Order order)
    {
        var repo = provider.GetRequiredService<IOrderRepository>();
        var sender = provider.GetRequiredService<IEmailSender>();
        // ...
    }
}

//  GOOD: Explicit constructor dependencies
public class OrderProcessor(IOrderRepository repo, IEmailSender sender)
{
    public void Process(Order order)
    {
        // ...
    }
}
```

---

### ❌ Anti-Pattern 3: Manual Disposal of Injected Services
**What it is:** Calling `.Dispose()` on a service that was provided to you via DI.
- **Impact:** Breaks other callers sharing the scoped/singleton instance (causing `ObjectDisposedException`).
- **Fix:** Let the container handle disposal at the end of the scope.

---

### ❌ Anti-Pattern 4: Asynchronous Deadlocks with `GetAwaiter().GetResult()` in Factories
**What it is:** Calling async methods synchronously inside registration delegates.
- **Impact:** Thread pool starvation and potential deadlocks during DI resolution.
- **Fix:** Keep registrations synchronous. Perform async initialization during application startup (e.g., via `IHostedService` or `IStartupFilter`).

---

# 8. Quick Reference Cheatsheet Table

| Goal | Configuration (Registration) | Consumption (Resolution) |
| :--- | :--- | :--- |
| **New instance every call** | `services.AddTransient<IService, Service>()` | `public Consumer(IService service)` |
| **One instance per HTTP Request** | `services.AddScoped<IService, Service>()` | `public Consumer(IService service)` |
| **One instance application-wide** | `services.AddSingleton<IService, Service>()` | `public Consumer(IService service)` |
| **Keyed Service (Named/Enum)** | `services.AddKeyedScoped<IService, Impl>("key")` | `public Consumer([FromKeyedServices("key")] IService svc)` |
| **Keyed Fallback / Wildcard** | `services.AddKeyedScoped<IService, DefaultImpl>(KeyedService.AnyKey)` | Resolves when an unregistered key is requested |
| **Factory Initialization** | `services.AddScoped<IService>(sp => new Service(...))` | `public Consumer(IService service)` |
| **Keyed Factory** | `services.AddKeyedScoped<IService>("k", (sp, key) => ...)` | `public Consumer([FromKeyedServices("k")] IService svc)` |
| **Open Generic** | `services.AddTransient(typeof(IRepo<>), typeof(Repo<>))` | `public Consumer(IRepo<Order> orderRepo)` |
| **All Implementations** | Multiple `services.AddTransient<IService, Impl*>()` | `public Consumer(IEnumerable<IService> services)` |
| **Conditional Registration** | `services.TryAddScoped<IService, Service>()` | Standard resolution |
| **Configuration Options** | `services.Configure<AppOpts>(config.GetSection("App"))` | `public Consumer(IOptions<AppOpts> options)` |
| **Scope inside Background Task** | N/A | `using var scope = scopeFactory.CreateScope();`<br>`var svc = scope.ServiceProvider.GetRequiredService<T>();` |
| **Blazor Component** | Standard registration | `@inject IService Service` or `[Inject(Key = "k")]` |
| **Minimal API Endpoint** | Standard registration | `app.MapGet("/", (IService svc, [FromKeyedServices("k")] IKeyed k) => ...)` |
