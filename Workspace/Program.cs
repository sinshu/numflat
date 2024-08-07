﻿using System;
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
        /*
        VectorExamples.Run();
        MatrixExamples.Run();
        OtherExamples.Run();
        */

        Vec<double>[] xs =
        [
            // Noise
            [-100, -100],

            // cluster 1
            [0, 0],
            [0, 1],
            [1, 0],
            [1, 1],

            // Noise
            [100, 100],

            // cluster 2
            [10, 0],
            [10, 1],
            [11, 0],
            [11, 1],

            // Noise
            [200, 200],

            // cluster 3
            [0, 10],
            [0, 11],
            [1, 10],
            [1, 11],

            // Noise
            [1000, 1000],
        ];

        var result = xs.DbScan(2, 3);
        foreach (var value in result)
        {
            Console.WriteLine(value);
        }
    }
}
