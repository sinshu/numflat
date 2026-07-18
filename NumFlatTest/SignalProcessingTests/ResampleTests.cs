using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using MathNet.Numerics.IntegralTransforms;
using NumFlat;
using NumFlat.IO;
using NumFlat.SignalProcessing;

namespace NumFlatTest.SignalProcessingTests
{
    public class ResampleTests
    {
        [TestCase(7, 1, 1)]
        [TestCase(8, 2, 3)]
        public void Upsample(int a, int srcStride, int dstStride)
        {
            var source = TestVector.RandomDouble(0, 30, srcStride);
            source.Clear();
            source[source.Count / 2] = 1;

            var destination = TestVector.RandomDouble(0, 300, dstStride);
            SignalProcessing.Resample(source, destination, 10, 1, a);

            for (var i = 0; i < destination.Count; i++)
            {
                var x = (i - destination.Count / 2) / 10.0;
                var expected = Lanczos(x, a);
                Assert.That(destination[i], Is.EqualTo(expected).Within(1.0E-12));
            }

            TestVector.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(20, 1, 1)]
        [TestCase(24, 3, 2)]
        public void Downsample(int period, int srcStride, int dstStride)
        {
            var source = TestVector.RandomDouble(0, 100, srcStride);
            for (var i = 0; i < source.Count; i++)
            {
                source[i] = Math.Sin(2 * Math.PI * i / period);
            }

            var destination = TestVector.RandomDouble(0, source.Count * 4 / period, dstStride);
            SignalProcessing.Resample(source, destination, 1, period / 4, 10);

            for (var i = 4; i < destination.Count; i += 4)
            {
                Assert.That(destination[i + 0], Is.EqualTo(0.0).Within(0.05));
                Assert.That(destination[i + 1], Is.EqualTo(1.0).Within(0.05));
                Assert.That(destination[i + 2], Is.EqualTo(0.0).Within(0.05));
                Assert.That(destination[i + 3], Is.EqualTo(-1.0).Within(0.05));
            }

            TestVector.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(100, 10, 2, 3, 1, 2)]
        [TestCase(100, 10, 3, 2, 2, 1)]
        [TestCase(50, 12, 4, 5, 3, 2)]
        [TestCase(50, 12, 5, 4, 2, 3)]
        public void Resample(int srcLength, int period, int p, int q, int srcStride, int dstStride)
        {
            var source = TestVector.RandomDouble(0, srcLength, srcStride);
            for (var i = 0; i < source.Count; i++)
            {
                source[i] = Math.Sin(2 * Math.PI * i / period);
            }

            var dstLength = srcLength * p / q;
            var destination = TestVector.RandomDouble(0, dstLength, dstStride);
            SignalProcessing.Resample(source, destination, p, q, 10);

            for (var i = 1; i < destination.Count - 1; i++)
            {
                var x = Math.Sin(2 * Math.PI * i / period * q / p);
                Assert.That(destination[i], Is.EqualTo(x).Within(0.05));
            }

            TestVector.FailIfOutOfRangeWrite(destination);
        }

        [Test]
        public void PassThrough()
        {
            var source = TestVector.RandomDouble(0, 50, 1);

            {
                var destination = TestVector.RandomDouble(0, 50, 1);
                SignalProcessing.Resample(source, destination, 100, 100, 10);
                NumAssert.AreSame(source, destination, 0);
            }

            {
                var destination = TestVector.RandomDouble(0, 10, 1);
                SignalProcessing.Resample(source, destination, 100, 100, 10);
                NumAssert.AreSame(source.Subvector(0, 10), destination, 0);
            }

            {
                var destination = TestVector.RandomDouble(0, 60, 1);
                SignalProcessing.Resample(source, destination, 100, 100, 10);
                NumAssert.AreSame(source, destination.Subvector(0, 50), 0);
                NumAssert.AreSame(destination.Subvector(50, 10), new Vec<double>(10), 0);
            }
        }

        [TestCase(48000, 16000, 3, 1)]
        [TestCase(16000, 48000, 1, 3)]
        public void CommonFactorsUseReducedRatio(int p, int q, int reducedP, int reducedQ)
        {
            var source = TestVector.RandomDouble(42, 30, 2);

            var actual = source.Resample(p, q, 10);
            var expected = source.Resample(reducedP, reducedQ, 10);

            NumAssert.AreSame(expected, actual, 0);
        }

        [TestCase(100, 10, 2, 4)]
        [TestCase(100, 10, 4, 2)]
        [TestCase(50, 12, 9, 5)]
        [TestCase(50, 12, 5, 9)]
        public void ExtensionMethod(int srcLength, int period, int p, int q)
        {
            var source = TestVector.RandomDouble(0, srcLength, 1);
            for (var i = 0; i < source.Count; i++)
            {
                source[i] = Math.Sin(2 * Math.PI * i / period);
            }

            var destination = source.Resample(p, q);

            for (var i = 5; i < destination.Count - 5; i++)
            {
                var x = Math.Sin(2 * Math.PI * i / period * q / p);
                Assert.That(destination[i], Is.EqualTo(x).Within(0.05));
            }

            TestVector.FailIfOutOfRangeWrite(destination);
        }

        [TestCase("noise16k.wav", "noise16kto48k.wav", 3, 1)]
        [TestCase("noise48k.wav", "noise48kto16k.wav", 1, 3)]
        public void MatchesPythonLanczosReferenceWave(string sourceFileName, string expectedFileName, int p, int q)
        {
            var (source, sourceSampleRate) = WaveFile.ReadMono(Path.Combine("dataset", sourceFileName));
            var (expected, expectedSampleRate) = WaveFile.ReadMono(Path.Combine("dataset", expectedFileName));

            var actual = source.Resample(p, q, 50);

            Assert.That(expectedSampleRate * q, Is.EqualTo(sourceSampleRate * p));
            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            NumAssert.AreSame(expected, actual, 1.0 / 32768 + 1.0E-12);
        }

        [TestCase(1, 12, 5, 1, 3, 1, 2)]
        [TestCase(2, 15, 4, 1, 2, 3, 1)]
        [TestCase(9, 7, 3, 1, 1, 2, 3)]
        [TestCase(1, 10, 1, 3, 2, 2, 1)]
        [TestCase(2, 8, 1, 4, 3, 1, 3)]
        [TestCase(17, 3, 1, 2, 1, 3, 2)]
        public void ConvolutionPathMatchesDirectCalculation(
            int sourceLength,
            int destinationLength,
            int p,
            int q,
            int a,
            int sourceStride,
            int destinationStride)
        {
            var source = TestVector.RandomDouble(42, sourceLength, sourceStride);
            var destination = TestVector.RandomDouble(0, destinationLength, destinationStride);

            SignalProcessing.Resample(source, destination, p, q, a);

            for (var i = 0; i < destination.Count; i++)
            {
                var expected = LocalResample(source, i, p, q, a);
                Assert.That(destination[i], Is.EqualTo(expected).Within(1.0E-12));
            }

            TestVector.FailIfOutOfRangeWrite(destination);
        }

        private static double LocalResample(in Vec<double> source, int destinationIndex, int p, int q, int a)
        {
            var position = (double)destinationIndex * q / p;
            var sincFactor = p > q ? 1.0 : (double)q / p;
            var gain = p < q ? (double)p / q : 1.0;
            var left = Math.Max(0, (int)Math.Floor(position - a * sincFactor) + 1);
            var right = Math.Min(source.Count, (int)Math.Ceiling(position + a * sincFactor));
            var sincScale = Math.PI / sincFactor;
            var windowScale = sincScale / a;

            var sum = 0.0;
            for (var i = left; i < right; i++)
            {
                var distance = i - position;
                sum += source[i] * LocalSinc(sincScale * distance) * LocalSinc(windowScale * distance);
            }
            return gain * sum;
        }

        private static double LocalSinc(double x)
        {
            return Math.Abs(x) < 1.0E-15 ? 1.0 : Math.Sin(x) / x;
        }

        private static double Lanczos(double x, int a)
        {
            if (Math.Abs(x) < a)
            {
                return Special.Sinc(x) * Special.Sinc(x / a);
            }
            else
            {
                return 0;
            }
        }
    }
}
