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
        VectorExamples.Run();
        MatrixExamples.Run();
        OtherExamples.Run();

        var frameLength = 1024;
        var frameShift = frameLength / 4;
        var window = WindowFunctions.Hann(frameLength);

        var piano = WaveFile.ReadMono("piano.wav").Data;
        var stft = piano.Stft(window, frameShift);
        var power = stft.Spectrogram.Select(spectrum => spectrum.Map(x => x.MagnitudeSquared())).ToArray();

        var (w, h) = NonnegativeMatrixFactorization.GetInitialGuess(power, 3);
        CsvFile.Write("w.csv", w);
    }
}
