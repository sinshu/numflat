using System;
using System.Collections.Generic;
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

        var sampleRate = 16000;
        var fftLength = 1024;
        var fb = new FilterBank(sampleRate, fftLength, 50, 7500, 15, FrequencyScale.Mel);

        using (var writer = new StreamWriter("test.csv"))
        {
            var sep = ",";
            foreach (var filter in fb.Filters)
            {
                for (var i = 0; i < filter.Coefficients.Count; i++)
                {
                    writer.WriteLine((filter.FrequencyBinStartIndex + i) + sep + filter.Coefficients[i]);
                }
                sep += ",";
            }
        }
    }
}
