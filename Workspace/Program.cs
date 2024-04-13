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

        var sampleRate = 16000;
        var fftLength = 1024;
        var filters = new TriangularFilter[10];
        var frequencies = new double[filters.Length + 2];
        var min = 50;
        var max = 700;
        for (var i = 0; i < frequencies.Length; i++)
        {
            frequencies[i] = min + (double)i / (frequencies.Length - 1) * (max - min);
        }
        for (var i = 0; i < filters.Length; i++)
        {
            var lower = frequencies[i];
            var center = frequencies[i + 1];
            var upper = frequencies[i + 2];
            if (i == 3)
            {
                Console.WriteLine();
            }
            filters[i] = new TriangularFilter(sampleRate, fftLength, lower, center, upper);
        }

        using (var writer = new StreamWriter("test.csv"))
        {
            var sep = ",";
            foreach (var filter in filters)
            {
                for (var i = 0; i < filter.Coefficients.Count; i++)
                {
                    writer.WriteLine((filter.FrequencyBinStartIndex + i) + sep + filter.Coefficients[i]);
                }
                sep += ",";
            }
        }

        var a = new double[fftLength / 2];
        foreach (var filter in filters)
        {
            for (var i = 0; i < filter.Coefficients.Count; i++)
            {
                a[filter.FrequencyBinStartIndex + i] += filter.Coefficients[i];
            }
        }
        var k = 0;
        foreach (var value in a)
        {
            var freq = (double)k / fftLength * sampleRate;
            Console.WriteLine(value + " " + freq);
            k++;
        }
    }
}
