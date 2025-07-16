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
using NumFlat.TimeSeries;

public class Program
{
    public static void Main(string[] args)
    {
        double[] xs = [1, 2, 3];
        double[] ys = [0, 0, 2, 2, 3, 0, 0, 1, 1, 2, 2, 3, 3, 0, 0, 0, 1, 2, 3, 0];
        var sdtw = new SubsequenceDynamicTimeWarping<double, double>(xs, ys, (x, y) => Math.Abs(x - y));
        Console.WriteLine(sdtw.CostMatrix);

        var path = sdtw.GetAlignment(0);
        foreach (var pair in path)
        {
            Console.WriteLine(pair);
        }

        Console.WriteLine("==");

        var best = SubsequenceDynamicTimeWarping.GetBestAlignment(xs, ys, (x, y) => Math.Abs(x - y));
        foreach (var pair in best)
        {
            Console.WriteLine(pair);
        }
    }
}
