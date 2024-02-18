using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NumFlat;
using NumFlat.Distributions;
using NumFlat.FourierTransform;
using NumFlat.MultivariateAnalyses;

public static class OtherExamples
{
    public static void Run()
    {
        Example1();
        Example2();
        Example3();
    }

    public static void Example1()
    {
        Console.WriteLine("=== OtherExample 1 ===");
        Console.WriteLine();

        // using NumFlat.MultivariateAnalyses;

        // Read some data.
        IEnumerable<Vec<double>> xs = ReadSomeData();

        // Do PCA.
        var pca = xs.Pca();

        foreach (var x in xs)
        {
            // Transform the vector based on the PCA.
            var transformed = pca.Transform(x);
        }

        // Read some label.
        IEnumerable<int> ys = ReadSomeLabel();

        // Do LDA.
        var lda = xs.Lda(ys);

        foreach (var x in xs)
        {
            // Transform the vector based on the LDA.
            var transformed = lda.Transform(x);
        }

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example2()
    {
        Console.WriteLine("=== OtherExample 2 ===");
        Console.WriteLine();

        // using NumFlat.Distributions;

        // Read some data.
        IEnumerable<Vec<double>> xs = ReadSomeData();

        // Compute the maximum likelihood Gaussian distribution.
        var gaussian = xs.ToGaussian();

        foreach (var x in xs)
        {
            // Compute the log PDF.
            var pdf = gaussian.LogPdf(x);
        }

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example3()
    {
        Console.WriteLine("=== OtherExample 2 ===");
        Console.WriteLine();

        // using NumFlat.FourierTransform;

        // Some complex vector.
        var samples = new Vec<Complex>(256);
        samples[0] = 1;

        // Do FFT.
        var spectrum = samples.Fft();

        // Do IFFT.
        samples = spectrum.Ifft();

        Console.WriteLine();
        Console.WriteLine();
    }

    private static IEnumerable<Vec<double>> ReadSomeData()
    {
        foreach (var line in File.ReadLines("iris.csV").Skip(1))
        {
            var split = line.Split(',');
            yield return split.Select(double.Parse).Take(4).ToVector();
        }
    }

    private static IEnumerable<int> ReadSomeLabel()
    {
        foreach (var line in File.ReadLines("iris.csV").Skip(1))
        {
            yield return line.Split(',')[4] switch
            {
                "setosa" => 0,
                "versicolor" => 1,
                "virginica" => 2,
                _ => throw new Exception()
            };
        }
    }
}
