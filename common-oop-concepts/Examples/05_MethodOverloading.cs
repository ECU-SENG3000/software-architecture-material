namespace DemoApp.Examples;

/// <summary>
/// Concept: Method overloading — same method name, different parameter lists,
/// resolved by the compiler at compile time. See CONCEPTS.md section 5.
/// </summary>
public static class MethodOverloadingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Calculator.Add overloads ===");
        var calc = new Calculator();
        Console.WriteLine(calc.Add(2, 3));             // int, int
        Console.WriteLine(calc.Add(2.5, 3.25));         // double, double
        Console.WriteLine(calc.Add(1, 2, 3));           // int, int, int
        Console.WriteLine(calc.Add(1, 2, 3, 4));        // params int[]

        Console.WriteLine();

        Console.WriteLine("=== Example 2: Printer.Print overloads (different types) ===");
        var printer = new Printer();
        printer.Print("Hello, class!");
        printer.Print(42);
        printer.Print(new[] { "a", "b", "c" });
    }
}

// Example 1: overloading based on parameter COUNT.
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public int Add(int a, int b, int c) => a + b + c;
    public int Add(params int[] numbers) => numbers.Sum();
}

// Example 2: overloading based on parameter TYPE.
public class Printer
{
    public void Print(string text) => Console.WriteLine($"String: {text}");
    public void Print(int number) => Console.WriteLine($"Int: {number}");
    public void Print(string[] items) => Console.WriteLine($"Array: [{string.Join(", ", items)}]");
}
