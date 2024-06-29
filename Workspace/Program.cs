using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using NumFlat;
using NumFlat.AudioFeatures;
using NumFlat.Clustering;
using NumFlat.Distributions;
using NumFlat.IO;
using NumFlat.MultivariateAnalyses;
using NumFlat.SignalProcessing;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExamples.Run();
        MatrixExamples.Run();
        OtherExamples.Run();

        var xs = Enumerable.Range(0, 100).Select(i => new Vec<double>(3)).ToArray();
        var random = new Random();
        foreach (var x in xs)
        {
            foreach (ref var value in x)
            {
                value = random.NextDouble();
            }
        }
        var pca = xs.Pca();
        Vec<double> test = [1, 2, 3];
        Vec<double> dst = new(2);
        pca.Transform(test, dst);
        Console.WriteLine(dst);
        Console.WriteLine(pca.Transform(test));
        Console.WriteLine(pca.InverseTransform(dst.Append(0.0).ToVector()));
        pca.InverseTransform(dst, test);
        Console.WriteLine(test);
    }
}
