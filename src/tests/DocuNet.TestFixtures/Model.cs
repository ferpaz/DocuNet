using System.Runtime.CompilerServices;

namespace Fixture;

/// <summary>A constructor that must never run while documenting.</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class ExplosiveAttribute : Attribute
{
    public ExplosiveAttribute() => throw new InvalidOperationException("ATTRIBUTE EXECUTED");
}

internal static class ModuleGuard
{
    [ModuleInitializer]
    internal static void Initialize() => throw new InvalidOperationException("MODULE EXECUTED");
}

/// <summary>The first class &lt;b&gt;literal&lt;/b&gt; &amp; text.</summary>
[Explosive]
public class First
{
    /// <summary>FIRST method.</summary>
    [Explosive]
    public void Same() { }
    /// <summary>OVERLOAD zero.</summary>
    public string Find() => "";
    /// <summary>OVERLOAD one.</summary><param name="id">Input identifier.</param><returns>Found value.</returns>
    public string Find(int id) => id.ToString();
    /// <summary>Generic method <paramref name="item"/>.</summary>
    public T Echo<T>(T item) => item;
    /// <summary>A property.</summary><value>Property value.</value>
    public int Id { get; private set; }
    /// <summary>A field.</summary>
    public string Name = "";
    /// <summary>A constructor that is omitted.</summary>
    public First() { }
    /// <summary>Not public.</summary>
    protected void Hidden() { }
    public void NoXml() { }
}

/// <summary>The second class.</summary>
public class Second
{
    /// <summary>SECOND method.</summary>
    public void Same() { }
}

/// <summary>Generic base.</summary>
public class Base<T>
{
    /// <summary>Inherited generic value.</summary>
    public T Value { get; set; } = default!;
    /// <summary>Inherited method.</summary>
    public T Use(T value) => value;
}

/// <summary>Derived generic binding.</summary>
public class Child : Base<string>;

/// <summary>A public interface.</summary>
public interface IContract { void Execute(); }

/// <summary>Generic shape.</summary>
public class Container<T>
{
    /// <summary>Nested enum.</summary>
    public enum Status { Ready }
    /// <summary>An indexer.</summary>
    public T this[int index] => default!;
    /// <summary>Byref and arrays.</summary>
    public void Complex(ref T value, out int count, int[,] matrix, T[] items) => count = 0;
}

/// <summary>Extensions.</summary>
public static class Extensions
{
    /// <summary>An extension.</summary>
    public static string Describe(this First value) => value.Name;
}

/// <summary>A value type.</summary>
public struct Point { public int X; }

internal class HiddenType { }
