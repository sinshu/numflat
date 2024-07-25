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

        Vec<double> x = [1, 2, 3, 4, 5];
        Console.WriteLine(x[1..^1]);

        Mat<double> mat =
        [
            [1, 2, 3],
            [2, 2, 3],
            [3, 2, 3],
            [4, 2, 3],
        ];

        Console.WriteLine(mat.Rows[1..^1]);
        Console.WriteLine(mat.Cols[..^1]);
    }
}
