using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.SignalProcessing;

namespace NumFlatTest.SignalProcessingTests
{
    public class StftTests
    {
        [TestCase(1000, 128, 64, 1, 1)]
        [TestCase(1000, 128, 64, 3, 2)]
        [TestCase(1500, 64, 16, 1, 1)]
        [TestCase(1500, 64, 16, 2, 3)]
        public void FftConsistency_AnalysisMode(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.Hann(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Analysis);

            for (var i = 0; i < spectrogram.Length; i++)
            {
                var position = info.GetFramePosition(i);
                var frame = source.GetWindowedFrameAsComplex(position, window);
                var expected = frame.Fft().Subvector(0, frameLength / 2 + 1);
                NumAssert.AreSame(expected, spectrogram[i], 1.0E-12);
            }
        }

        [TestCase(1000, 128, 64, 1, 1)]
        [TestCase(1000, 128, 64, 3, 2)]
        [TestCase(1500, 64, 16, 1, 1)]
        [TestCase(1500, 64, 16, 2, 3)]
        public void FftConsistency_SynthesisMode(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.Hann(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);

            for (var i = 0; i < spectrogram.Length; i++)
            {
                var position = info.GetFramePosition(i);
                var frame = source.GetWindowedFrameAsComplex(position, window);
                var expected = frame.Fft().Subvector(0, frameLength / 2 + 1);
                NumAssert.AreSame(expected, spectrogram[i], 1.0E-12);
            }
        }

        [TestCase(1000, 128, 32, 1, 1)]
        [TestCase(1000, 128, 32, 3, 2)]
        [TestCase(500, 128, 16, 1, 1)]
        [TestCase(500, 128, 16, 2, 3)]
        public void ReconstructionHann(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.Hann(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);

            var reconstructed = spectrogram.Istft(info);

            NumAssert.AreSame(source, reconstructed, 1.0E-12);
        }

        [TestCase(1000, 128, 32, 1, 1)]
        [TestCase(1000, 128, 32, 3, 2)]
        [TestCase(500, 128, 16, 1, 1)]
        [TestCase(500, 128, 16, 2, 3)]
        public void ReconstructionHamming(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.Hamming(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);

            var reconstructed = spectrogram.Istft(info);

            NumAssert.AreSame(source, reconstructed, 1.0E-12);
        }

        [TestCase(1000, 128, 64, 1, 1)]
        [TestCase(1000, 128, 64, 3, 2)]
        public void ReconstructionHannSquared(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.HannSquared(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);

            var reconstructed = spectrogram.Istft(info);

            NumAssert.AreSame(source, reconstructed, 1.0E-12);
        }
    }
}
