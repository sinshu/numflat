using System;
using System.Linq;
using NumFlat;
using NumFlat.IO;
using NumFlat.SignalProcessing;
using ScottPlot;

public static class ScottPlotExamples
{
    public static void Spectrogram()
    {
        var frameLength = 1024;
        var frameShift = frameLength / 2;
        var window = WindowFunctions.Hann(frameLength);

        var (source, sampleRate) = WaveFile.ReadMono("piano.wav");

        var (spectrogram, info) = source.Stft(window, frameShift);

        var data = spectrogram.ColsToMatrix().Map(x => Math.Log(x.MagnitudeSquared() + 1.0E-6)).ToArray();

        using (var plot = new Plot())
        {
            var signal = plot.Add.Signal(source);
            signal.AlwaysUseLowDensityMode = true;
            signal.Color = Colors.Blue;
            signal.Data.Period = 1.0 / sampleRate;
            plot.XLabel("Time [s]");
            plot.YLabel("Amplitude");
            plot.Layout.Fixed(new PixelPadding(75, 25, 50, 25));
            plot.SavePng("waveform.png", 800, 300);
        }

        using (var plot = new Plot())
        {
            var heatmap = plot.Add.Heatmap(data);
            heatmap.FlipVertically = true;
            heatmap.CellWidth = (double)frameShift / sampleRate;
            heatmap.CellHeight = (double)sampleRate / frameLength;
            heatmap.CellAlignment = Alignment.LowerLeft;
            heatmap.Colormap = new ScottPlot.Colormaps.Inferno();
            heatmap.Smooth = true;
            plot.XLabel("Time [s]");
            plot.YLabel("Frequency [Hz]");
            plot.Layout.Fixed(new PixelPadding(75, 25, 50, 25));
            plot.SavePng("spectrogram.png", 800, 300);
        }
    }
}
