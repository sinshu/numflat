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
        /*
        VectorExamples.Run();
        MatrixExamples.Run();
        OtherExamples.Run();
        */

        Vec<double> coefficients = [1, 2, 3];
        var intercept = 4.0;

        var random = new Random(42);
        var xs = new List<Vec<double>>();
        var ys = new List<double>();
        for (var i = 0; i < 50; i++)
        {
            var x = Enumerable.Range(0, 3).Select(k => random.NextGaussian()).ToVector();
            var y = coefficients * x + intercept;
            xs.Add(x);
            ys.Add(y);
        }

        var regression = new LinearRegression(xs, ys, 0);
        foreach (var (x, y) in xs.Zip(ys))
        {
            Console.WriteLine(y + " => " + regression.Transform(x));
        }
        Console.WriteLine(regression.Intercept);

        Console.WriteLine(ys.Average());
    }
}
