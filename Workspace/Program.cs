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

        var random = new Random(42);

        var xs1 = new List<Vec<double>>();
        var ys1 = new List<int>();
        for (var i = 0; i < 50; i++)
        {
            xs1.Add([random.NextGaussian(), random.NextGaussian()]);
            ys1.Add(0);
        }

        var xs2 = new List<Vec<double>>();
        var ys2 = new List<int>();
        for (var i = 0; i < 50; i++)
        {
            xs1.Add([random.NextGaussian() + 1, random.NextGaussian() - 1]);
            ys2.Add(1);
        }

        var xs = xs1.Concat(xs2).ToArray();
        var ys = ys1.Concat(ys2).ToArray();
        new LogisticRegression(xs, ys);
    }
}
