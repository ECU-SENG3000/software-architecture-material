namespace DemoApp.Examples;

/// <summary>
/// Concept: Delegates — Action (no return value) and Func (returns a value).
/// See CONCEPTS.md section 8.
/// </summary>
public static class DelegatesDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Action delegates (methods that return void) ===");
        Action<string> logToConsole = message => Console.WriteLine($"[LOG] {message}");
        Action<string> shout = message => Console.WriteLine(message.ToUpper() + "!!!");

        RunNotification("Class is starting", logToConsole);
        RunNotification("Pop quiz", shout);

        Console.WriteLine();

        Console.WriteLine("=== Example 2: Func delegates (methods that return a value) ===");
        Func<int, int, int> add = (a, b) => a + b;
        Func<int, bool> isEven = n => n % 2 == 0;

        Console.WriteLine($"add(3, 4) = {add(3, 4)}");
        var numbers = new[] { 1, 2, 3, 4, 5, 6 };
        var evens = numbers.Where(isEven).ToList(); // Func passed straight into LINQ
        Console.WriteLine($"Evens: [{string.Join(", ", evens)}]");
    }

    // Accepting an Action<string> parameter means "call me back with a string, expect nothing returned".
    private static void RunNotification(string message, Action<string> notify) => notify(message);
}
