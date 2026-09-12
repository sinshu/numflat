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
        [TestCase(128, 128, 64, 1)]
        [TestCase(129, 128, 64, 1)]
        [TestCase(191, 128, 64, 1)]
        [TestCase(192, 128, 64, 2)]
        [TestCase(193, 128, 64, 2)]
        [TestCase(1000, 128, 64, 14)]
        [TestCase(1500, 64, 16, 90)]
        [TestCase(256, 128, 128, 2)]
        public void FrameCount_AnalysisMode(int sourceLength, int frameLength, int frameShift, int expectedCount)
        {
            var source = TestVector.RandomDouble(42, sourceLength, 1);
            var window = WindowFunctions.Hann(frameLength);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Analysis);

            Assert.That(spectrogram.Length, Is.EqualTo(expectedCount));
            Assert.That(info.GetFramePosition(0).Start, Is.Zero);
            Assert.That(info.GetFramePosition(expectedCount - 1).End, Is.LessThanOrEqualTo(sourceLength));
            Assert.That(info.GetFramePosition(expectedCount).End, Is.GreaterThan(sourceLength));
        }

        [TestCase(1, 32)]
        [TestCase(64, 32)]
        [TestCase(65, 64)]
        [TestCase(127, 64)]
        [TestCase(127, 128)]
        public void InsufficientSource_AnalysisMode(int sourceLength, int frameShift)
        {
            var source = TestVector.RandomDouble(42, sourceLength, 1);
            var window = WindowFunctions.Hann(128);

            var exception = Assert.Throws<ArgumentException>(new Action(() => source.Stft(window, frameShift, StftMode.Analysis)));

            Assert.That(exception!.ParamName, Is.EqualTo("source"));
        }

        [TestCase(1, 2)]
        [TestCase(63, 2)]
        [TestCase(64, 2)]
        [TestCase(65, 3)]
        [TestCase(128, 3)]
        [TestCase(129, 4)]
        public void FrameCount_SynthesisMode(int sourceLength, int expectedCount)
        {
            var source = TestVector.RandomDouble(42, sourceLength, 1);
            var window = WindowFunctions.SquareRootHann(128);

            var (spectrogram, info) = source.Stft(window, 64, StftMode.Synthesis);

            Assert.That(spectrogram.Length, Is.EqualTo(expectedCount));
            Assert.That(info.FirstFramePosition, Is.EqualTo(-64));
            NumAssert.AreSame(source, spectrogram.Istft(info), 1.0E-12);
        }

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
                Assert.That(position.Start + window.Count, Is.EqualTo(position.End));
                Assert.That(position.Start + window.Count / 2, Is.EqualTo(position.Center));

                var frame = source.GetWindowedFrameAsComplex(position.Start, window);
                var expected = frame.Fft().Subvector(0, frameLength / 2 + 1);
                NumAssert.AreSame(expected, spectrogram[i], 1.0E-12);
            }
        }

        [TestCase(1333, 64, 16, 1, 1)]
        [TestCase(1333, 64, 16, 2, 3)]
        public void FftConsistency_SynthesisMode(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.Hann(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);

            for (var i = 0; i < spectrogram.Length; i++)
            {
                var position = info.GetFramePosition(i);
                Assert.That(position.Start + window.Count, Is.EqualTo(position.End));
                Assert.That(position.Start + window.Count / 2, Is.EqualTo(position.Center));

                var frame = source.GetWindowedFrameAsComplex(position.Start, window);
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
        [TestCase(543, 128, 16, 1, 1)]
        [TestCase(543, 128, 16, 2, 3)]
        public void ReconstructionHamming(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.Hamming(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);
            var reconstructed = spectrogram.Istft(info);

            NumAssert.AreSame(source, reconstructed, 1.0E-12);
        }

        [TestCase(980, 128, 64, 1, 1)]
        [TestCase(980, 128, 64, 3, 2)]
        public void ReconstructionSquareRootHann(int sourceLength, int frameLength, int frameShift, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, srcStride);
            var window = TestVector.RandomDouble(0, frameLength, winStride);
            WindowFunctions.SquareRootHann(frameLength).CopyTo(window);

            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);
            var reconstructed = spectrogram.Istft(info);

            NumAssert.AreSame(source, reconstructed, 1.0E-12);
        }

        [Test]
        public void GetFrameTime()
        {
            var sampleRate = 16000;
            var frameLength = 1024;
            var frameShift = frameLength / 2;
            var source = TestVector.RandomDouble(42, 5000, 1);
            var window = WindowFunctions.SquareRootHann(frameLength);
            var (spectrogram, info) = source.Stft(window, frameShift, StftMode.Synthesis);

            Assert.That(info.GetFrameTime(sampleRate, 0).Start, Is.EqualTo((double)-frameShift / sampleRate).Within(1.0E-12));
            Assert.That(info.GetFrameTime(sampleRate, 1).Start, Is.EqualTo(0.0).Within(1.0E-12));
            Assert.That(info.GetFrameTime(sampleRate, 2).Start, Is.EqualTo((double)frameShift / sampleRate).Within(1.0E-12));

            Assert.That(info.GetFrameTime(sampleRate, 0).End, Is.EqualTo((double)frameShift / sampleRate).Within(1.0E-12));
            Assert.That(info.GetFrameTime(sampleRate, 1).End, Is.EqualTo((double)(2 * frameShift) / sampleRate).Within(1.0E-12));
            Assert.That(info.GetFrameTime(sampleRate, 2).End, Is.EqualTo((double)(3 * frameShift) / sampleRate).Within(1.0E-12));

            Assert.That(info.GetFrameTime(sampleRate, 0).Center, Is.EqualTo(0.0).Within(1.0E-12));
            Assert.That(info.GetFrameTime(sampleRate, 1).Center, Is.EqualTo((double)frameShift / sampleRate).Within(1.0E-12));
            Assert.That(info.GetFrameTime(sampleRate, 2).Center, Is.EqualTo((double)(2 * frameShift) / sampleRate).Within(1.0E-12));
        }

        [Test]
        public void GetFrequency()
        {
            var source = TestVector.RandomDouble(42, 5000, 1);
            var window = WindowFunctions.Hann(1024);
            var (spectrogram, info) = source.Stft(window, 512, StftMode.Analysis);
            Assert.That(info.GetFrequency(16000, 256), Is.EqualTo(4000).Within(1.0E-12));
        }
    }
}
