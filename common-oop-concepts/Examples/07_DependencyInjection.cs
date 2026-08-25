using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.Examples;

/// <summary>
/// Concept: Dependency Injection / Inversion of Control — classes declare what they need
/// (via interfaces) instead of creating it themselves; a container wires everything up.
/// See CONCEPTS.md section 7.
/// </summary>
public static class DependencyInjectionDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Manual DI (constructor injection, no container) ===");
        // We choose the implementation here and "inject" it via the constructor.
        IGreeter consoleGreeter = new ConsoleGreeter();
        var manualService = new WelcomeService(consoleGreeter);
        manualService.Welcome("Ada");

        Console.WriteLine();

        Console.WriteLine("=== Example 2: Microsoft.Extensions.DependencyInjection container ===");
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, EnthusiasticGreeter>();
        services.AddTransient<WelcomeService>();

        using var provider = services.BuildServiceProvider();
        // The container figures out that WelcomeService needs an IGreeter and supplies it.
        var containerService = provider.GetRequiredService<WelcomeService>();
        containerService.Welcome("Grace");
    }
}

// The "contract" that any greeter implementation must fulfill.
public interface IGreeter
{
    string Greet(string name);
}

public class ConsoleGreeter : IGreeter
{
    public string Greet(string name) => $"Hello, {name}.";
}

public class EnthusiasticGreeter : IGreeter
{
    public string Greet(string name) => $"HEY {name.ToUpper()}!! Great to see you!!";
}

// WelcomeService depends on IGreeter (an interface), never on a concrete class.
// It has no idea whether it will receive a ConsoleGreeter, EnthusiasticGreeter, or a test fake.
public class WelcomeService(IGreeter greeter)
{
    public void Welcome(string name) => Console.WriteLine(greeter.Greet(name));
}
