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

        var n = 50;
        var source = new Vec<double>(30);
        source[source.Count / 2] = 1;
        var destination = new Vec<double>(n * source.Count);
        SignalProcessing.Resample(source, destination, n, 1, 10);
        CsvFile.Write("out.csv", destination);
    }
}
