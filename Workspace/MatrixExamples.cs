using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat;

public static class MatrixExamples
{
    public static void Run()
    {
        Example1();
        Example2();
        Example3();
    }

    public static void Example1()
    {
        Console.WriteLine("=== MatrixExample 1 ===");
        Console.WriteLine();

        // Creat a new matrix.
        var array = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        };

        var matrix = array.ToMatrix();

        // Show the matrix.
        Console.WriteLine(matrix);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example2()
    {
        Console.WriteLine("=== MatrixExample 2 ===");
        Console.WriteLine();

        // Creat a new matrix.
        var matrix = new Mat<double>(3, 3);

        // Element-wise access.
        for (var row = 0; row < matrix.RowCount; row++)
        {
            for (var col = 0; col < matrix.ColCount; col++)
            {
                matrix[row, col] = 10 * row + col;
            }
        }

        // Show the matrix.
        Console.WriteLine(matrix);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example3()
    {
        Console.WriteLine("=== MatrixExample 3 ===");
        Console.WriteLine();

        // Some matrices.
        var x = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        }
        .ToMatrix();

        var y = new double[,]
        {
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 3, 6, 9 },
        }
        .ToMatrix();

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

        Console.WriteLine();
        Console.WriteLine();
    }
}
