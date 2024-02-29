using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NumFlat;
using NumFlat.Clustering;
using NumFlat.Distributions;
using NumFlat.SignalProcessing;
using NumFlat.MultivariateAnalyses;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExamples.Run();
        MatrixExamples.Run();
        OtherExamples.Run();

        var random = new Random(42);
        var signal = Enumerable.Range(0, 100).Select(i => random.NextDouble()).ToVector();
        var window = Enumerable.Repeat(1.0, 8).ToVector();
        var (stft, info) = signal.Stft(window, window.Count / 2, StftMode.Synthesis);
        Console.WriteLine(stft[0]);

        var frame = signal.GetWindowedFrameAsComplex(-4, window);
        Console.WriteLine(frame.Fft());
    }
}
