using System.Text.Json;

namespace DemoApp.Examples;

/// <summary>
/// Concept: Serializing a POCO (Plain Old CLR Object) to JSON and back using System.Text.Json.
/// See CONCEPTS.md section 9.
/// </summary>
public static class JsonSerializationDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Serialize a POCO to a JSON string ===");
        var student = new Student("Ada Lovelace", 22, ["C#", "Python"]);
        string json = JsonSerializer.Serialize(student);
        Console.WriteLine(json);

        Console.WriteLine();

        Console.WriteLine("=== Example 2: Deserialize a JSON string back into a POCO ===");
        string incomingJson = """{"Name":"Grace Hopper","Age":45,"Languages":["COBOL","FORTRAN"]}""";
        var deserialized = JsonSerializer.Deserialize<Student>(incomingJson);
        Console.WriteLine($"{deserialized!.Name} ({deserialized.Age}) knows: {string.Join(", ", deserialized.Languages)}");

        Console.WriteLine();

        Console.WriteLine("=== Example 3: Deserialize a JSON array into a List<Student> ===");
        string incomingJsonArray = """
            [
                {"Name":"Katherine Johnson","Age":33,"Languages":["FORTRAN"]},
                {"Name":"Margaret Hamilton","Age":29,"Languages":["Assembly"]}
            ]
            """;
        var roster = JsonSerializer.Deserialize<List<Student>>(incomingJsonArray)!;
        foreach (var classmate in roster)
        {
            Console.WriteLine($"{classmate.Name} ({classmate.Age}) knows: {string.Join(", ", classmate.Languages)}");
        }
    }
}

// A POCO: a plain class with just properties, no base class or framework baggage required.
public class Student(string name, int age, List<string> languages)
{
    public string Name { get; set; } = name;
    public int Age { get; set; } = age;
    public List<string> Languages { get; set; } = languages;
}
