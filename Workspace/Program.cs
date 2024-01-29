using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NumFlat;

public class Program
{
    public static void Main(string[] args)
    {
        var mat = new Mat<double>(20, 20);
        for (var row = 0; row < mat.RowCount; row++)
        {
            for (var col = 0; col < mat.ColCount; col++)
            {
                mat[row, col] = row + col / 100.0;
            }
        }

        Console.WriteLine(mat);
    }
}
