using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NumFlat;

public class Program
{
    public static void Main(string[] args)
    {
        var x = new Mat<float>(3, 5);
        var y = new Mat<float>(3, 5);
        for (var row = 0; row < x.RowCount; row++)
        {
            for (var col = 0; col < x.ColCount; col++)
            {
                x[row, col] = row + col / 100.0F;
                y[row, col] = row + col;
            }
        }

        Console.WriteLine(x);
        Console.WriteLine(y);

        var destination = new Mat<float>(5, 5);
        Mat.Mul(x, true, y, false, destination);
        Console.WriteLine(destination);
    }
}
