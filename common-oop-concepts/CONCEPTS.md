# C# Architecture Concepts — Student Reference

This document explains the core object-oriented and C# language concepts used in this
project. Each concept has **two runnable examples** in the `DemoApp/Examples` folder.
Run the project (`dotnet run` from the `DemoApp` folder) to see all examples execute
in order, with console output labeled to match this document.

If you're coming from Python, the biggest mental shift is that C# is **statically typed**
and **compiled** — the compiler checks types and member access before the program ever runs.

---

## 1. Objects (Properties, Methods, Visibility)

An **object** is an instance of a **class** — a blueprint that bundles *data* (fields/properties)
and *behavior* (methods) together.

- **Properties** are named pieces of data exposed by an object (similar to a Python attribute,
  but can have custom `get`/`set` logic). Examples use property constructs directly instead of
  manually declaring a backing field:
  - **Auto-implemented property**: `public string Owner { get; }` — the compiler generates the
    hidden backing field for you.
  - **Auto-implemented property with a private setter**: `public decimal Balance { get; private set; }`
    — readable by anyone, but only settable from inside the class (this is how you get
    controlled/"encapsulated" mutation without writing your own field).
  - **Expression-bodied (computed) property**: `public string Name => "Circle";` — no field at
    all, the value is computed every time it's read.
  - **Read/write auto-property**: `public double Width { get; set; }` — anyone can read or write it.
- **Methods** are functions that belong to the object and usually operate on its data.
- **Visibility** (access modifiers) controls who can see/use a member:
  - `public` — anyone can access it.
  - `private` — only code inside the same class can access it (default if omitted).
  - `protected` — the class itself and any subclasses can access it.
  - `internal` — anything in the same project/assembly can access it.

Python doesn't enforce this at all (everything is "public by convention" with `_underscore`
naming), so pay attention: in C#, `private` is a real compiler-enforced wall.

File: `DemoApp/Examples/01_Objects.cs`

---

## 2. Interfaces

An **interface** is a contract: a list of methods/properties a class *promises* to implement,
with no implementation of its own (similar in spirit to a Python `Protocol` or an
`abc.ABC` with only abstract methods, but interfaces are checked at compile time).

Interfaces let you write code that depends on *behavior* ("anything that can `Speak()`")
instead of a specific concrete class. This is the foundation of loosely-coupled design and
is used everywhere in Dependency Injection (see section 7).

File: `DemoApp/Examples/02_Interfaces.cs`

---

## 3. Inheritance

**Inheritance** lets a class (the *derived*/*child* class) reuse and extend the members of
another class (the *base*/*parent* class) using `: BaseClass`. It models an **"is-a"**
relationship (a `Dog` *is an* `Animal`).

- `base.Method()` calls the parent's implementation.
- `virtual` on a base method allows a child to `override` it.
- `sealed` prevents further inheritance or overriding.

File: `DemoApp/Examples/03_Inheritance.cs`

---

## 4. Polymorphism

**Polymorphism** ("many forms") means you can treat different derived types through a common
base type/interface reference, and the *correct* overridden method runs automatically at
runtime — this is called **dynamic dispatch**.

This is what makes interfaces and inheritance actually useful: you can write one method that
accepts `Animal` (or `IAnimal`) and it works correctly no matter which concrete subtype
(`Dog`, `Cat`, etc.) is passed in.

File: `DemoApp/Examples/04_Polymorphism.cs`

---

## 5. Method Overloading

**Overloading** lets you define multiple methods with the **same name** but different
**parameter lists** (different number, order, or types of parameters). The compiler picks
the best match at compile time based on the arguments you pass — this is called
**static/compile-time dispatch** (contrast with polymorphism, which is runtime dispatch).

Python doesn't support this natively (only the last-defined function with a given name wins),
so this is a genuinely new capability.

File: `DemoApp/Examples/05_MethodOverloading.cs`

---

## 6. Extension Methods (Old and New Technique)

**Extension methods** let you "add" a method to an existing type (even one you don't own,
like `string` or `List<T>`) without modifying its source code or creating a subclass.

- **Old technique (C# 3.0+, still the most common style today):** a `static` method in a
  `static` class, where the first parameter is prefixed with `this`.
- **New technique (C# 14 "extension" members, available in .NET 10):** an `extension(...)`
  block inside a static class that groups multiple extension members (methods, properties)
  for the same type together, with cleaner syntax.

File: `DemoApp/Examples/06_ExtensionMethods.cs`

---

## 7. Dependency Injection (Inversion of Control)

**Dependency Injection (DI)** is a pattern where a class declares *what it needs* (usually
via constructor parameters typed as **interfaces**) instead of creating those dependencies
itself with `new`. Something else — a **DI container** — is responsible for constructing
the object graph and "injecting" the right implementations.

This flips the direction of control: instead of `MyClass` deciding which `ILogger`
implementation to use, the container decides and hands it to `MyClass`. That's why it's
also called **Inversion of Control (IoC)**. Benefits: easier testing (swap in a fake/mock),
easier swapping of implementations, and looser coupling between classes.

This project uses the official `Microsoft.Extensions.DependencyInjection` container.

File: `DemoApp/Examples/07_DependencyInjection.cs`

---

## 8. Delegates (`Action` and `Func`)

A **delegate** is a type-safe reference to a method — essentially "a variable that holds a
function", similar to passing a function as a value in Python.

- **`Action`** delegates point to methods that return **void** (`Action`, `Action<T>`,
  `Action<T1,T2>`, ...).
- **`Func`** delegates point to methods that **return a value**, where the **last** type
  parameter is always the return type (`Func<TResult>`, `Func<T, TResult>`, ...).

Delegates are the foundation of C# lambdas/callbacks and event handling, and are used
heavily in LINQ (`.Where(x => ...)`, `.Select(x => ...)`).

File: `DemoApp/Examples/08_Delegates.cs`

---

## 9. Serializing a POCO to JSON

A **POCO** ("Plain Old CLR Object") is just a regular class with properties — no base class,
attributes, or framework dependency required to use it. `System.Text.Json` (built into .NET)
can convert a POCO instance to a JSON string (`Serialize`) and parse a JSON string back into
an instance (`Deserialize<T>`), matching JSON property names to your class's property names.
Deserialization also works with a JSON array, producing a `List<T>` of POCOs in one call.

File: `DemoApp/Examples/09_JsonSerialization.cs`

---

## 10. Builder Pattern

The **Builder pattern** separates the *step-by-step construction* of an object from its final
form, usually through a **fluent API** (methods that return `this`/the builder so calls can be
chained: `builder.Step1().Step2().Build()`).

.NET uses this pattern heavily for application startup — `WebApplicationBuilder` (ASP.NET Core)
and `DistributedApplicationBuilder` (.NET Aspire) both follow the same shape:

1. `var builder = XyzApplication.CreateBuilder(args);` — get a builder.
2. Configure it: `builder.Services.AddXyz(...)`.
3. `var app = builder.Build();` — produce the final, immutable app object.
4. `app.Run();` — start it.

File: `DemoApp/Examples/10_BuilderPattern.cs`
