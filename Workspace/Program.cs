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

        Vec<double> mean = [1, 2, 3];
        Mat<double> cov =
        [
            [4, 1, 2],
            [1, 5, 3],
            [2, 3, 6],
        ];
        var gaussian = new Gaussian(mean, cov);
        var random = new Random(42);
        var xs = new List<Vec<double>>();
        for (var i = 0; i < 10000; i++)
        {
            xs.Add(gaussian.Generate(random));
        }
        Console.WriteLine(xs.Mean());
        Console.WriteLine(xs.Covariance());
    }
}
