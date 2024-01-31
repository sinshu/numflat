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
            new []{ 1.0, 2.0, 3.0 },
            new []{ 4.0, 5.0, 6.0 },
            new []{ 7.0, 8.0, 9.0 },
            new []{ 10.0, 11.0, 12.0 },
        };

        Console.WriteLine(arr.RowsToMatrix());
        Console.WriteLine(arr.ColsToMatrix());
    }

    private static void VectorExample1()
    {
        // Creat a new vector.
        Vec<double> vector = [1, 2, 3];

        // Element-wise access.
        vector[0] = 4;
        vector[1] = 5;
        vector[2] = 6;

        // Show the vector.
        Console.WriteLine(vector);
    }

    private static void VectorExample2()
    {
        var array = new double[] { 1, 2, 3 };
        var enumerable = Enumerable.Range(0, 3).Select(i => i + 1.0);

        // Create a vector from an array.
        var vectorFromArray = array.ToVector();

        // Create a vector from an enumerable.
        var vectorFromEnumerable = enumerable.ToVector();
    }

    private static void VectorExample3()
    {
        // Creat a new vector.
        Vec<double> vector = [1, 2, 3];

        // Create a subvector.
        var subvector = vector.Subvector(1, 2);

        // Modify the subvector.
        subvector[0] = 100;

        // Show the original vector.
        Console.WriteLine(vector);
    }

    private static void VectorExample4()
    {
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

        // Dot product.
        var dot = x * y;

        // Outer product.
        var outer = x.Outer(y);
    }
}
