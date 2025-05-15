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
        var positions = new List<Vec<double>>();
        var random = new Random(42);
        var n = 20;

        for (var i = 0; i < n; i++)
        {
            positions.Add([random.NextGaussian(), random.NextGaussian()]);
        }

        var d = new Mat<double>(n, n);
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                d[i, j] = positions[i].Distance(positions[j]);
            }
        }

        var x = ClassicalMultiDimensionalScaling.Fit(d, 2);

        Console.WriteLine(d);

        Console.WriteLine(positions.RowsToMatrix());

        Console.WriteLine(x);

        var d2 = new Mat<double>(n, n);
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                d2[i, j] = x.Rows[i][0..2].Distance(x.Rows[j][0..2]);
            }
        }

        Console.WriteLine(d2);
    }
}
