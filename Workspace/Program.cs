using System;
using System.Collections.Generic;
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

        var ys = new List<Vec<double>>();
        var n = 300;
        for (var i = 0; i < n; i++)
        {
            var value1 = Math.Sin(2 * Math.PI * i / n * 11);
            var value2 = Math.Sign(Math.Cos(2 * Math.PI * i / n * 7));
            var value3 = (i % 13) / 13.0 * 2 - 1;
            ys.Add([value1, value2, value3]);
        }

        Mat<double> transform =
        [
            [3, 6, 5],
            [5, 2, 7],
            [6, 4, 4],
            [1, 2, 3],
        ];

        var xs = ys.Select(y => transform * y).ToArray();

        using (var writer = new StreamWriter("ys.csv"))
        {
            foreach (var y in ys)
            {
                writer.WriteLine(string.Join(',', y));
            }
        }

        using (var writer = new StreamWriter("xs.csv"))
        {
            foreach (var x in xs)
            {
                writer.WriteLine(string.Join(',', x));
            }
        }

        var ica = new IndependentComponentAnalysis(xs, 3);

        using (var writer = new StreamWriter("result.csv"))
        {
            foreach (var x in xs)
            {
                writer.WriteLine(string.Join(',', ica.Transform(x)));
            }
        }
    }
}
