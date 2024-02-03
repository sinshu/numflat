using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat;

public static class VectorExamples
{
    public static void Run()
    {
        Example1();
        Example2();
        Example3();
        Example4();
    }

    public static void Example1()
    {
        Console.WriteLine("=== VectorExample 1 ===");
        Console.WriteLine();

        // Creat a new vector.
        Vec<double> vector = [1, 2, 3];

        // Show the vector.
        Console.WriteLine(vector);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example2()
    {
        Console.WriteLine("=== VectorExample 2 ===");
        Console.WriteLine();

        // Some enumerable.
        var enumerable = Enumerable.Range(0, 10).Select(i => i / 10.0);

        // Create a vector from an enumerable.
        var vector = enumerable.ToVector();

        // Show the vector.
        Console.WriteLine(vector);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example3()
    {
        Console.WriteLine("=== VectorExample 3 ===");
        Console.WriteLine();

        // Creat a new vector.
        var vector = new Vec<double>(3);

        // Element-wise access.
        vector[0] = 4;
        vector[1] = 5;
        vector[2] = 6;

        // Show the vector.
        Console.WriteLine(vector);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example4()
    {
        Console.WriteLine("=== VectorExample 4 ===");
        Console.WriteLine();

        // Some vectors.
        Vec<double> x = [1, 2, 3];
        Vec<double> y = [4, 5, 6];

        // Addition.
        var add = x + y;

        // Subtraction.
        var sub = x - y;

        // Multiplication by a scalar.
        var mul = x * 3;

        // Division by a scalar.
        var div = x / 3;

        // Pointwise multiplication.
        var pm = x.PointwiseMul(y);

        // Pointwise division.
        var pd = x.PointwiseDiv(y);

        // Dot product.
        var dot = x * y;

        // Outer product.
        var outer = x.Outer(y);

        Console.WriteLine();
        Console.WriteLine();
    }
}
