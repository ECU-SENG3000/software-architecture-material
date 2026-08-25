namespace DemoApp.Examples;

/// <summary>
/// Concept: Interfaces — a contract of members with no implementation.
/// See CONCEPTS.md section 2.
/// </summary>
public static class InterfacesDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: INotifier (contract for sending a message) ===");
        INotifier notifier = new EmailNotifier();
        notifier.Send("Class starts at 9am!");
        notifier = new SmsNotifier(); // swap implementation, calling code doesn't change
        notifier.Send("Class starts at 9am!");

        Console.WriteLine();

        Console.WriteLine("=== Example 2: IShape (contract implemented by multiple shapes) ===");
        IShape[] shapes = [new Circle(3), new Square(4)];
        foreach (var shape in shapes)
        {
            Console.WriteLine($"{shape.Name} area = {shape.Area():F2}");
        }
    }
}

// Example 1: an interface describing "anything that can send a notification".
public interface INotifier
{
    void Send(string message);
}

public class EmailNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"[Email] {message}");
}

public class SmsNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"[SMS] {message}");
}

// Example 2: an interface describing "anything that has a name and an area".
public interface IShape
{
    string Name { get; }
    double Area();
}

public class Circle(double radius) : IShape
{
    public string Name => "Circle";
    public double Area() => Math.PI * radius * radius;
}

public class Square(double side) : IShape
{
    public string Name => "Square";
    public double Area() => side * side;
}
