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
        var random = new Random(42);
        var dim = 4;
        var n = 10;
        var xs = new List<Vec<double>>();
        for (var i = 0; i < n; i++)
        {
            var elements = Enumerable.Range(0, dim).Select(i => random.NextDouble());
            var x = elements.ToVector();
            xs.Add(x);
        }

        var dst = new Mat<double>(dim, dim);
        var mean = new Vec<double>(dim);
        MathLinq.Covariance(xs, mean, dst, 1);
        Console.WriteLine(dst);
    }
}
