// Software Architecture course demos — see CONCEPTS.md for explanations of each topic.
using DemoApp.Examples;

RunSection("1. OBJECTS", ObjectsDemo.Run);
// RunSection("2. INTERFACES", InterfacesDemo.Run);
// RunSection("3. INHERITANCE", InheritanceDemo.Run);
// RunSection("4. POLYMORPHISM", PolymorphismDemo.Run);
// RunSection("5. METHOD OVERLOADING", MethodOverloadingDemo.Run);
// RunSection("6. EXTENSION METHODS", ExtensionMethodsDemo.Run);
// RunSection("7. DEPENDENCY INJECTION", DependencyInjectionDemo.Run);
// RunSection("8. DELEGATES (Action / Func)", DelegatesDemo.Run);
// RunSection("9. JSON SERIALIZATION", JsonSerializationDemo.Run);
// RunSection("10. BUILDER PATTERN", BuilderPatternDemo.Run);

static void RunSection(string title, Action demo)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 60));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 60));
    demo();
}
