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

        var sampleRate = 16000;
        var frameLength = 1024;
        var frameShift = 512;
        var window = WindowFunctions.SquareRootHann(frameLength);

        var source = WaveFile.ReadMono("piano.wav").Data;
        var spectrogram = source.Stft(window, frameShift).Spectrogram;
        //spectrogram.RowsToMatrix
    }
}
