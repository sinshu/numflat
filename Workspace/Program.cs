using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NumFlat;
using NumFlat.MultivariateAnalyses;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExamples.Run();
        MatrixExamples.Run();

        var data = ReadWithLabel().ToArray();
        var xs = data.Select(pair => pair.Item1).ToArray();
        var ys = data.Select(pair => pair.Item2).ToArray();

        var lda = new LinearDiscriminantAnalysis(xs, ys);

        using (var writer = new StreamWriter("out.csv"))
        {
            writer.WriteLine("1st,2nd");
            foreach (var x in xs)
            {
                var transformed = lda.Transform(x);
                writer.WriteLine(transformed[0] + "," + transformed[1]);
                Console.WriteLine(x);
            }
        }
    }

    private static IEnumerable<Vec<double>> Read()
    {
        foreach (var line in File.ReadLines("iris.csv").Skip(1))
        {
            yield return line.Split(',').Take(4).Select(value => double.Parse(value)).ToVector();
        }
    }

    private static IEnumerable<(Vec<double>, int)> ReadWithLabel()
    {
        foreach (var line in File.ReadLines("iris.csv").Skip(1))
        {
            var split = line.Split(',');
            int label;
            switch (split[4])
            {
                case "setosa": label = 0; break;
                case "versicolor": label = 1; break;
                case "virginica": label = 2; break;
                default: throw new Exception();
            }
            var vec = line.Split(',').Take(4).Select(value => double.Parse(value)).ToVector();
            yield return (vec, label);
        }
    }
}
