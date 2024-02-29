using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NumFlat;
using NumFlat.Clustering;
using NumFlat.Distributions;
using NumFlat.MultivariateAnalyses;
using NumFlat.SignalProcessing;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExamples.Run();
        MatrixExamples.Run();
        OtherExamples.Run();
    }
}
