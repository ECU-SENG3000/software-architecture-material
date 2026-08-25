namespace DemoApp.Examples;

/// <summary>
/// Concept: Objects — properties, methods, and visibility (access modifiers).
/// See CONCEPTS.md section 1.
/// </summary>
public static class ObjectsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: BankAccount (properties + visibility) ===");
        var account = new BankAccount("Ada Lovelace", 100m);
        account.Deposit(50m);
        account.Withdraw(30m);
        Console.WriteLine($"{account.Owner}'s balance: {account.Balance:C}");
        // account.Balance = 1000000m; // Would NOT compile: the setter is private.

        Console.WriteLine();

        Console.WriteLine("=== Example 2: Rectangle (methods that use properties) ===");
        var rect = new Rectangle(4, 5);
        Console.WriteLine($"Rectangle {rect.Width}x{rect.Height} -> Area={rect.Area()}, Perimeter={rect.Perimeter()}");
        rect.Width = 10; // public setter allows this
        Console.WriteLine($"After resizing width -> Area={rect.Area()}");
    }
}

// Example 1: a class with public properties (no manual backing fields) and public methods.
public class BankAccount
{
    // public properties: the safe, controlled "front door" to the data.
    public string Owner { get; }

    // auto-implemented property with a private setter: only this class can change it (visibility).
    public decimal Balance { get; private set; }

    public BankAccount(string owner, decimal openingBalance)
    {
        Owner = owner;
        Balance = openingBalance;
    }

    // public methods: behavior that operates on the object's own data.
    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > Balance) throw new InvalidOperationException("Insufficient funds.");
        Balance -= amount;
    }
}

// Example 2: a simple class with public read/write properties and methods that derive values.
public class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Area() => Width * Height;
    public double Perimeter() => 2 * (Width + Height);
}
