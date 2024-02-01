using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using NumFlat;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExample1();
        VectorExample2();
        VectorExample3();
        VectorExample4();

        var arr = new double[][]
        {
            new double[] { 1, 2, 3 },
            new double[] { 0, 1, 2 },
            new double[] { 0, 0, 1 },
        };

        var mat = arr.RowsToMatrix();
        Console.WriteLine(mat);
        var inv = mat.Inverse();
        Console.WriteLine(mat);
        Console.WriteLine(inv);

        using var mem = System.Buffers.MemoryPool<int>.Shared.Rent(30);
        Console.WriteLine(mem.Memory.Length);
    }

    private static void VectorExample1()
    {
        Console.WriteLine("=== VectorExample 1 ===");

        // Creat a new vector.
        Vec<double> vector = [1, 2, 3];

        // Element-wise access.
        vector[0] = 4;
        vector[1] = 5;
        vector[2] = 6;

        // Show the vector.
        Console.WriteLine(vector);

        Console.WriteLine();
    }

    private static void VectorExample2()
    {
        Console.WriteLine("=== VectorExample 2 ===");

        var array = new double[] { 1, 2, 3 };
        var enumerable = Enumerable.Range(0, 3).Select(i => i + 1.0);

        // Create a vector from an array.
        var vectorFromArray = array.ToVector();

        // Create a vector from an enumerable.
        var vectorFromEnumerable = enumerable.ToVector();

        Console.WriteLine("vectorFromArray: " + vectorFromArray);
        Console.WriteLine("vectorFromEnumerable: " + vectorFromEnumerable);

        Console.WriteLine();
    }

    private static void VectorExample3()
    {
        Console.WriteLine("=== VectorExample 3 ===");

        // Creat a new vector.
        Vec<double> vector = [1, 2, 3];

        // Create a subvector.
        var subvector = vector.Subvector(1, 2);

        // Modify the subvector.
        subvector[0] = 100;

        // Show the original vector.
        Console.WriteLine(vector);

        Console.WriteLine();
    }

    private static void VectorExample4()
    {
        Console.WriteLine("=== VectorExample 4 ===");

        // Creat new vectors.
        Vec<double> x = [1, 2, 3];
        Vec<double> y = [4, 5, 6];

        // Addition.
        var add = x + y;

        // Subtraction.
        var sub = x - y;

        // Multiplication by a scalar.
        var sm = x * 3;

        // Division by a scalar.
        var sd = y / 3;

        // Pointwise multiplication.
        var pm = x.PointwiseMul(y);

        // Pointwise division.
        var pd = x.PointwiseDiv(y);

        // Dot product (returns a scalar).
        var dot = x * y;

        // Outer product (returns a matrix).
        var outer = x.Outer(y);

        Console.WriteLine("add: " + add);
        Console.WriteLine("sub: " + sub);
        Console.WriteLine("sm: " + sm);
        Console.WriteLine("sd: " + sd);
        Console.WriteLine("pm: " + pm);
        Console.WriteLine("pd: " + pd);
        Console.WriteLine("dot: " + dot);
        Console.WriteLine("outer: " + outer);
        Console.WriteLine();
    }
}
