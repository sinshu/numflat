using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NumFlat;
using NumFlat.FourierTransform;
using NumFlat.MultivariateAnalyses;

public class Program
{
    public static void Main(string[] args)
    {
        var test = new Vec<Complex>(8);
        test[0] = 1;
        test.FftInplace();
        Console.WriteLine(test);
    }
}
