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

        var source = WaveFile.ReadMono(@"C:\Users\sinsh\Desktop\kikuri01.wav");
        var resampled = source.Data.Resample(48000, 44100);
        WaveFile.Write("out.wav", resampled, 48000);
    }
}
