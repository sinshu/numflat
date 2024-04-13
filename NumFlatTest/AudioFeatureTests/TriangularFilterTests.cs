using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.AudioFeatures;

namespace NumFlatTest.ClusteringTests
{
    public class TriangularFilterTests
    {
        [TestCase(16000, 1024, 100, 200, 400, 1)]
        [TestCase(16000, 1024, 57.0, 57.1, 57.2, 2)]
        [TestCase(44100, 2048, 15000, 16000, 17000, 3)]
        [TestCase(8000, 512, 0, 2000, 4000, 4)]
        public void SingleFilter(int sampleRate, int fftLength, double lowerFreq, double centerFreq, double upperFreq, int stride)
        {
            var filter = new TriangularFilter(sampleRate, fftLength, lowerFreq, centerFreq, upperFreq);
            Assert.That(filter.SampleRate, Is.EqualTo(sampleRate));
            Assert.That(filter.FftLength, Is.EqualTo(fftLength));
            Assert.That(filter.LowerFrequency, Is.EqualTo(lowerFreq));
            Assert.That(filter.CenterFrequency, Is.EqualTo(centerFreq));
            Assert.That(filter.UpperFrequency, Is.EqualTo(upperFreq));

            var lowerW = lowerFreq / sampleRate * fftLength;
            var centerW = centerFreq / sampleRate * fftLength;
            var upperW = upperFreq / sampleRate * fftLength;

            var binStartIndex = (int)lowerW;
            Assert.That(filter.FrequencyBinStartIndex, Is.EqualTo(binStartIndex));

            var binEndIndex = (int)Math.Ceiling(upperW);
            var binCount = binEndIndex - binStartIndex;
            Assert.That(filter.Coefficients.Count, Is.EqualTo(binCount));

            foreach (var value in filter.Coefficients)
            {
                Assert.That(0.0 <= value && value <= 1.0);
            }

            for (var w = (int)lowerW; w < centerW - 1; w++)
            {
                var value1 = filter.Coefficients[w - filter.FrequencyBinStartIndex];
                var value2 = filter.Coefficients[w + 1 - filter.FrequencyBinStartIndex];
                Assert.That(value1 < value2);
            }

            for (var w = (int)centerW; w < upperW - 1; w++)
            {
                var value1 = filter.Coefficients[w - filter.FrequencyBinStartIndex];
                var value2 = filter.Coefficients[w + 1 - filter.FrequencyBinStartIndex];
                Assert.That(value1 > value2);
            }

            var spectrum = TestVector.RandomDouble(42, fftLength, stride);
            var expected = spectrum
                .Subvector(filter.FrequencyBinStartIndex, filter.Coefficients.Count)
                .Zip(filter.Coefficients, (x, y) => x * y)
                .Sum();
            var actual = filter.GetValue(spectrum);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(16000, 1024, 0, 8000, 10)]
        [TestCase(8000, 512, 100, 3000, 20)]
        [TestCase(8000, 512, 1000, 1500, 50)]
        public void MultipleFilters(int sampleRate, int fftLength, double minFreq, double maxFreq, int filterCount)
        {
            var freqCount = filterCount + 2;
            var freqs = new double[freqCount];
            for (var i = 0; i < freqs.Length; i++)
            {
                freqs[i] = minFreq + (double)i / (freqs.Length - 1) * (maxFreq - minFreq);
            }

            var filters = new TriangularFilter[filterCount];
            for (var i = 0; i < filters.Length; i++)
            {
                filters[i] = new TriangularFilter(sampleRate, fftLength, freqs[i], freqs[i + 1], freqs[i + 2]);
            }

            var filterSum = new Vec<double>(fftLength);
            foreach (var filter in filters)
            {
                filterSum.Subvector(filter.FrequencyBinStartIndex, filter.Coefficients.Count).AddInplace(filter.Coefficients);
            }

            var startIndex = (int)Math.Ceiling(filters.First().CenterFrequency / sampleRate * fftLength);
            var endIndex = (int)(filters.Last().CenterFrequency / sampleRate * fftLength);
            for (var i = startIndex; i < endIndex; i++)
            {
                Assert.That(filterSum[i], Is.EqualTo(1.0).Within(1.0E-12));
            }
        }
    }
}
