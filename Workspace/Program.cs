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

        var frameLength = 1024;
        var frameShift = frameLength / 2;
        var window = WindowFunctions.SquareRootHann(frameLength);

        var piano = WaveFile.ReadMono("piano.wav").Data;
        var stft = piano.Stft(window, frameShift);
        var power = stft.Spectrogram.Select(spectrum => spectrum.Map(x => x.MagnitudeSquared())).ToArray();

        var (w, h) = NonnegativeMatrixFactorization.GetInitialGuess(power, 3);
        for (var i = 0; i < 200; i++)
        {
            Console.WriteLine(i);
            var w2 = new Mat<double>(w.RowCount, w.ColCount);
            var h2 = new Mat<double>(h.RowCount, h.ColCount);
            NonnegativeMatrixFactorization.Update(power, w, h, w2, h2);
            w = w2;
            h = h2;
        }

        CsvFile.Write("w.csv", w);
        CsvFile.Write("h.csv", h.Transpose());
    }
}
