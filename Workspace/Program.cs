using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
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

        Vec<double> t = [1, 2, 3];
        Console.WriteLine(t);
    }
}
