namespace DemoApp.Examples;

/// <summary>
/// Concept: The Builder pattern — separate the step-by-step construction of a complex object
/// from its final representation, often using a fluent (chainable) API.
/// See CONCEPTS.md section 10.
/// </summary>
public static class BuilderPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Classic fluent builder (chained method calls) ===");
        Pizza pizza = new PizzaBuilder()
            .WithSize("Large")
            .AddTopping("Mozzarella")
            .AddTopping("Pepperoni")
            .Build();
        Console.WriteLine(pizza);

        Console.WriteLine();

        Console.WriteLine("=== Example 2: .NET-style app builder (CreateBuilder / Build / Run) ===");
        // This mirrors the shape of WebApplicationBuilder / DistributedApplicationBuilder:
        // 1) CreateBuilder(args) gives you a builder, 2) configure it, 3) Build(), 4) Run().
        GreeterAppBuilder builder = GreeterAppBuilder.CreateBuilder(args: []);
        builder.Services.AddSingleton("Welcome to Software Architecture!");
        builder.Services.AddSingleton("Don't forget the reading is due Friday.");

        GreeterApp app = builder.Build();
        app.Run();
    }
}

// Example 1: a POCO produced by the builder, plus a fluent builder class.
public class Pizza
{
    public string Size { get; set; } = "Medium";
    public List<string> Toppings { get; } = [];

    public override string ToString() => $"{Size} pizza with: {string.Join(", ", Toppings)}";
}

public class PizzaBuilder
{
    private readonly Pizza pizza = new();

    // Each method configures one piece, then returns "this" so calls can be chained.
    public PizzaBuilder WithSize(string size)
    {
        pizza.Size = size;
        return this;
    }

    public PizzaBuilder AddTopping(string topping)
    {
        pizza.Toppings.Add(topping);
        return this;
    }

    public Pizza Build() => pizza;
}

// Example 2: a tiny stand-in for the pattern used by WebApplicationBuilder / DistributedApplicationBuilder.
// Real ASP.NET Core / Aspire builders work the same way, just with far more configuration options.
public class GreeterAppBuilder
{
    // A minimal stand-in for the DI container exposed as builder.Services in ASP.NET Core.
    public GreeterServiceCollection Services { get; } = new();

    private GreeterAppBuilder() { }

    public static GreeterAppBuilder CreateBuilder(string[] args) => new();

    public GreeterApp Build() => new(Services.Messages);
}

public class GreeterServiceCollection
{
    public List<string> Messages { get; } = [];

    public GreeterServiceCollection AddSingleton(string message)
    {
        Messages.Add(message);
        return this;
    }
}

public class GreeterApp(List<string> messages)
{
    public void Run()
    {
        foreach (var message in messages)
        {
            Console.WriteLine($"[GreeterApp] {message}");
        }
    }
}
