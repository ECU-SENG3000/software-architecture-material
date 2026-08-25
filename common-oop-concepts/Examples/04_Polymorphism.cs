namespace DemoApp.Examples;

/// <summary>
/// Concept: Polymorphism — one reference type, many runtime behaviors (dynamic dispatch).
/// See CONCEPTS.md section 4.
/// </summary>
public static class PolymorphismDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: PaymentMethod list processed polymorphically ===");
        List<PaymentMethod> payments =
        [
            new CreditCardPayment("4111-xxxx"),
            new PayPalPayment("student@example.com"),
            new CashPayment()
        ];

        foreach (var payment in payments)
        {
            // Same method call, different behavior depending on the ACTUAL runtime type.
            payment.Process(25.00m);
        }

        Console.WriteLine();

        Console.WriteLine("=== Example 2: One method, many shapes via IShape (see Examples 02) ===");
        IShape[] shapes = [new Circle(2), new Square(5)];
        PrintAreas(shapes);
    }

    // This method doesn't know or care about concrete shape types — it just needs "an IShape".
    private static void PrintAreas(IEnumerable<IShape> shapes)
    {
        foreach (var shape in shapes)
        {
            Console.WriteLine($"{shape.Name}: {shape.Area():F2}");
        }
    }
}

// Example 1: an abstract base class with a virtual method, overridden differently per subtype.
public abstract class PaymentMethod
{
    public abstract void Process(decimal amount);
}

public class CreditCardPayment(string cardNumber) : PaymentMethod
{
    public override void Process(decimal amount) =>
        Console.WriteLine($"Charging {amount:C} to credit card {cardNumber}");
}

public class PayPalPayment(string email) : PaymentMethod
{
    public override void Process(decimal amount) =>
        Console.WriteLine($"Charging {amount:C} via PayPal account {email}");
}

public class CashPayment : PaymentMethod
{
    public override void Process(decimal amount) =>
        Console.WriteLine($"Accepting {amount:C} in cash");
}
