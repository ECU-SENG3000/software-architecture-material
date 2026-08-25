namespace DemoApp.Examples;

/// <summary>
/// Concept: Inheritance — a class reuses and extends members of a base class.
/// See CONCEPTS.md section 3.
/// </summary>
public static class InheritanceDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Animal -> Dog / Cat (base class reuse) ===");
        var dog = new Dog("Rex");
        var cat = new Cat("Whiskers");
        dog.Describe();
        cat.Describe();

        Console.WriteLine();

        Console.WriteLine("=== Example 2: Employee -> Manager (extending with new members) ===");
        var manager = new Manager("Grace Hopper", 120_000m, teamSize: 8);
        manager.Describe();
        Console.WriteLine(manager.GetAnnualBonus());
    }
}

// Example 1: base class with shared state/behavior; derived classes add or override behavior.
public class Animal
{
    public string Name { get; }

    public Animal(string name) => Name = name;

    public virtual string MakeSound() => "...";

    public void Describe() => Console.WriteLine($"{Name} says {MakeSound()}");
}

public class Dog(string name) : Animal(name)
{
    public override string MakeSound() => "Woof!";
}

public class Cat(string name) : Animal(name)
{
    public override string MakeSound() => "Meow!";
}

// Example 2: a derived class that adds a brand-new member (TeamSize) beyond the base class.
public class Employee(string name, decimal salary)
{
    public string Name { get; } = name;
    public decimal Salary { get; } = salary;

    public virtual void Describe() => Console.WriteLine($"{Name} earns {Salary:C}/yr");
}

public class Manager(string name, decimal salary, int teamSize) : Employee(name, salary)
{
    public int TeamSize { get; } = teamSize;

    // "override" + "base." lets the child reuse the parent's logic and add to it.
    public override void Describe()
    {
        base.Describe();
        Console.WriteLine($"{Name} manages a team of {TeamSize}.");
    }

    public string GetAnnualBonus() => $"{Name}'s bonus: {Salary * 0.1m:C}";
}
