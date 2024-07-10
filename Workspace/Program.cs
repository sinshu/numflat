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

        Mat<double> mat =
        [
            [1, 2, 3, 100],
            [4, 5, 6, 101],
            [7, 8, 9, 102],
        ];

        foreach (ref var value in mat.EnumerateDiagonalElements())
        {
            Console.WriteLine(value);
            value += 300;
        }

        Console.WriteLine(mat);
    }
}
