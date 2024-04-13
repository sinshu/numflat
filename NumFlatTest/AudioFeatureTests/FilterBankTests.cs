using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.AudioFeatures;

namespace NumFlatTest.AufioFeatureTests
{
    public class FilterBankTests
    {
        [TestCase(16000, 1024, 50, 7500, 10)]
        [TestCase(16000, 1024, 0, 8000, 20)]
        [TestCase(44100, 2048, 300, 20000, 15)]
        public void Linear(int sampleRate, int fftLength, double minFreq, double maxFreq, int count)
        {
            var fb = new FilterBank(sampleRate, fftLength, minFreq, maxFreq, count, FrequencyScale.Linear);
            Assert.That(fb.SampleRate, Is.EqualTo(sampleRate));
            Assert.That(fb.FftLength, Is.EqualTo(fftLength));
            Assert.That(fb.Filters.First().LowerFrequency, Is.EqualTo(minFreq).Within(1.0E-6));
            Assert.That(fb.Filters.Last().UpperFrequency, Is.EqualTo(maxFreq).Within(1.0E-6));
            Assert.That(fb.Filters.Count, Is.EqualTo(count));

            var firstPair = fb.Filters.Chunk(2).First();
            var d = firstPair[1].CenterFrequency - firstPair[0].CenterFrequency;

            foreach (var pair in fb.Filters.Chunk(2).Where(p => p.Length == 2))
            {
                Assert.That(pair[1].LowerFrequency - pair[0].LowerFrequency, Is.EqualTo(d).Within(1.0E-6));
                Assert.That(pair[1].CenterFrequency - pair[0].CenterFrequency, Is.EqualTo(d).Within(1.0E-6));
                Assert.That(pair[1].UpperFrequency - pair[0].UpperFrequency, Is.EqualTo(d).Within(1.0E-6));
            }
        }

        [TestCase(16000, 1024, 50, 7500, 10)]
        [TestCase(16000, 1024, 0, 8000, 20)]
        [TestCase(44100, 2048, 300, 20000, 15)]
        public void Mel(int sampleRate, int fftLength, double minFreq, double maxFreq, int count)
        {
            var fb = new FilterBank(sampleRate, fftLength, minFreq, maxFreq, count, FrequencyScale.Mel);
            Assert.That(fb.SampleRate, Is.EqualTo(sampleRate));
            Assert.That(fb.FftLength, Is.EqualTo(fftLength));
            Assert.That(fb.Filters.First().LowerFrequency, Is.EqualTo(minFreq).Within(1.0E-6));
            Assert.That(fb.Filters.Last().UpperFrequency, Is.EqualTo(maxFreq).Within(1.0E-6));
            Assert.That(fb.Filters.Count, Is.EqualTo(count));

            var firstPair = fb.Filters.Chunk(2).First();
            var pd1 = firstPair[1].UpperFrequency - firstPair[0].UpperFrequency;
            var pd2 = firstPair[1].CenterFrequency - firstPair[0].CenterFrequency;
            var pd3 = firstPair[1].LowerFrequency - firstPair[0].LowerFrequency;
            foreach (var pair in fb.Filters.Chunk(2).Skip(1).Where(p => p.Length == 2))
            {
                var d1 = pair[1].UpperFrequency - pair[0].UpperFrequency;
                var d2 = pair[1].CenterFrequency - pair[0].CenterFrequency;
                var d3 = pair[1].LowerFrequency - pair[0].LowerFrequency;
                Assert.That(d1 / pd1 > 1.2);
                Assert.That(d2 / pd2 > 1.2);
                Assert.That(d3 / pd3 > 1.2);
            }
        }
    }
}
