using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NumFlat;
using NumFlat.MultivariateAnalyses;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExamples.Run();
        MatrixExamples.Run();

        var xs = Read().ToArray();
        var pca = xs.Pca();

        Console.WriteLine();

        using (var writer = new StreamWriter("out.csv"))
        {
            writer.WriteLine("1st,2nd");
            foreach (var x in xs)
            {
                var transformed = pca.Transform(x);
                writer.WriteLine(transformed[0] + "," + transformed[1]);
            }
        }
    }

    private static IEnumerable<Vec<double>> Read()
    {
        foreach (var line in File.ReadLines("iris.csv").Skip(1))
        {
            yield return line.Split(',').Take(4).Select(value => double.Parse(value)).ToVector();
        }
    }
}
