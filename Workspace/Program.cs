using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using NumFlat;
using NumFlat.AudioFeatures;
using NumFlat.Clustering;
using NumFlat.Distributions;
using NumFlat.IO;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;
using NumFlat.SignalProcessing;
using NumFlat.TimeSeries;

public class Program
{
    public static void Main(string[] args)
    {
        Vec<double> x = [0, 1, 2, 3];
        var json = JsonSerializer.Serialize(x);
        Console.WriteLine(json);

        var options = NumFlatJsonSerializerOptions.Create();
        var y = JsonSerializer.Deserialize<Vec<double>>(json, options);
        Console.WriteLine(y);
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
