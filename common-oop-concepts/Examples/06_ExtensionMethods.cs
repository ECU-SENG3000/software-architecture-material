namespace DemoApp.Examples;

/// <summary>
/// Concept: Extension methods — adding methods to an existing type without modifying it.
/// Shows the classic (C# 3.0) technique and the new C# 14 "extension" block technique.
/// See CONCEPTS.md section 6.
/// </summary>
public static class ExtensionMethodsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: OLD technique - 'this' parameter static method ===");
        string word = "level";
        Console.WriteLine($"'{word}'.IsPalindromeOld() = {word.IsPalindromeOld()}");
        Console.WriteLine($"'hello'.IsPalindromeOld() = {"hello".IsPalindromeOld()}");

        int[] numbers = [4, 8, 15, 16, 23, 42];
        Console.WriteLine($"numbers.AverageOrZeroOld() = {numbers.AverageOrZeroOld()}");

        Console.WriteLine();

        Console.WriteLine("=== Example 2: NEW technique - C# 14 'extension' block ===");
        Console.WriteLine($"'{word}'.IsPalindromeNew() = {word.IsPalindromeNew()}");
        Console.WriteLine($"'hello'.IsPalindromeNew() = {"hello".IsPalindromeNew()}");
        Console.WriteLine($"numbers.AverageOrZeroNew() = {numbers.AverageOrZeroNew()}");
    }
}

// Example 1 (OLD, classic technique since C# 3.0):
// A static method in a static class, with "this" before the first parameter's type.
public static class OldStyleExtensions
{
    public static bool IsPalindromeOld(this string value)
    {
        var cleaned = value.ToLowerInvariant();
        return cleaned.SequenceEqual(cleaned.Reverse());
    }

    public static double AverageOrZeroOld(this int[] values) =>
        values.Length == 0 ? 0 : values.Average();
}

// Example 2 (NEW technique, C# 14 / .NET 10): an "extension" block groups members for a type.
public static class NewStyleExtensions
{
    extension(string value)
    {
        public bool IsPalindromeNew()
        {
            var cleaned = value.ToLowerInvariant();
            return cleaned.SequenceEqual(cleaned.Reverse());
        }
    }

    extension(int[] values)
    {
        public double AverageOrZeroNew() => values.Length == 0 ? 0 : values.Average();
    }
}
