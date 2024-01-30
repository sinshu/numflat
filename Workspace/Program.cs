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

        Console.WriteLine(x);
        Console.WriteLine(y);

        var destination = new Mat<float>(5, 5);
        Mat.Mul(x, y, destination, true, false);
        Console.WriteLine(destination);

        var cv = new DenseVector(3);
        cv[0] = new Complex(0.3, 0.5);
        cv[1] = new Complex(0.1, 0.4);
        cv[2] = new Complex(0.5, 0.1);
        Console.WriteLine(cv.OuterProduct(cv));

        var fv = new Vec<Complex>(cv.ToArray());
        var fm = new Mat<Complex>(fv.Count, fv.Count);
        Vec.Outer(fv, fv, fm, false);
        Console.WriteLine(fm.ToString());
    }
}
