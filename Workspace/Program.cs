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
using NumFlat.TimeSeries;

public class Program
{
    public static void Main(string[] args)
    {
        var sampleRate = 16000;
        var frameLength = 1024;
        var frameShift = frameLength / 2;
        var window = WindowFunctions.SquareRootHann(frameLength);

        var piano = WaveFile.ReadMono("piano.wav").Data;
        var srcStft = piano.Stft(window, frameShift);
        var power = srcStft.Spectrogram.Select(spectrum => spectrum.Map(x => x.MagnitudeSquared())).ToArray();

        var nmf = power.Nmf(3, null, new Random(42));
        nmf.W.Cols[1].Clear();
        var reconstructed = nmf.W * nmf.H;

        var dstStft = new List<Vec<Complex>>();
        foreach (var (rcn, org) in reconstructed.Cols.Zip(srcStft.Spectrogram))
        {
            var x = rcn.Zip(org, (r, o) => Complex.FromPolarCoordinates(Math.Sqrt(r), o.Phase)).ToVector();
            dstStft.Add(x);
        }

        var dst = dstStft.Istft(srcStft.Info);
        var max = dst.Select(x => Math.Abs(x)).Max();
        dst = 0.999 * dst / max;
        WaveFile.Write("out.wav", dst, sampleRate);
    }

    private static IEnumerable<Vec<double>> ReadIris(string filename)
    {
        foreach (var line in File.ReadLines(filename).Skip(1))
        {
            var values = line.Split(',').Take(4).Select(value => double.Parse(value));
            yield return values.ToVector();
        }
    }
}
