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
        var data = ReadIris("iris.csv").ToArray();
        var kernel = Kernel.Polynomial(2, 1);

        var kpca = new KernelPrincipalComponentAnalysis(data, kernel);

        using (var writer = new StreamWriter("out.csv"))
        {
            writer.WriteLine("x,y");
            foreach (var x in data)
            {
                var transformed = kpca.Transform(x);
                writer.WriteLine($"{transformed[0]},{transformed[1]}");
            }
        }
    }

    private static IEnumerable<Vec<double>> ReadIris(string filename)
    {
        foreach (var line in File.ReadLines(filename).Skip(1))
        {
            var values = line.Split(',').Take(4).Select(value => double.Parse(value));
            yield return values.ToVector();
        }
    }
}
