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

        Vec<double> w1 = [1, 2, 3, 0, 0];
        Vec<double> w2 = [0, 3, 1, 4, 0];
        Vec<double> w3 = [0, 0, 1, 3, 7];
        Mat<double> w = [w1, w2, w3];
        w = w.Transpose();
        Console.WriteLine(w);

        var n = 100;

        var h = new Mat<double>(w.ColCount, n);
        var random = new Random(42);
        foreach (var col in h.Cols)
        {
            col.MapInplace(i => 3 * random.NextDouble());
        }
        Console.WriteLine(h);

        var v = w * h;
        Console.WriteLine(v);

        var srcW = MatrixBuilder.FromFunc(w.RowCount, w.ColCount, (row, col) => random.NextDouble());
        var srcH = MatrixBuilder.FromFunc(h.RowCount, h.ColCount, (row, col) => random.NextDouble());
        //w.CopyTo(srcW);
        //h.CopyTo(srcH);
        var dstW = new Mat<double>(w.RowCount, w.ColCount);
        var dstH = new Mat<double>(h.RowCount, h.ColCount);
        for (var i = 0; i < 1000; i++)
        {
            NonnegativeMatrixFactorization.Update(v.Cols, srcW, srcH, dstW, dstH);
            dstW.CopyTo(srcW);
            dstH.CopyTo(srcH);
            Console.WriteLine(dstW);
            //Console.ReadKey();
        }

        CsvFile.Write("ref.csv", h.Transpose());
        CsvFile.Write("nmf.csv", dstH.Transpose());
    }
}
