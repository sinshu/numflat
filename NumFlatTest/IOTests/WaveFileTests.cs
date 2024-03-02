using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.IO;

namespace NumFlatTest.IOTests
{
    public class WaveFileTests
    {
        [Test]
        public void Sin1000Hz_16bit()
        {
            var (data, sampleRate) = WaveFile.Read(Path.Combine("dataset", "sin1000hz_16bit.wav"));

            Assert.That(data.Length == 1);
            Assert.That(data[0].Count == 1600);
            Assert.That(sampleRate == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[0][i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
            }
        }

        [Test]
        public void Sin1000Hz_32bit()
        {
            var (data, sampleRate) = WaveFile.Read(Path.Combine("dataset", "sin1000hz_32bit.wav"));

            Assert.That(data.Length == 1);
            Assert.That(data[0].Count == 1600);
            Assert.That(sampleRate == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[0][i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }
        }

        [Test]
        public void Sin1000Hz2000Hz_16bit()
        {
            var (data, sampleRate) = WaveFile.Read(Path.Combine("dataset", "sin1000hz2000hz_16bit.wav"));

            Assert.That(data.Length == 2);
            Assert.That(data[0].Count == 1600);
            Assert.That(data[1].Count == 1600);
            Assert.That(sampleRate == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[0][i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
            }

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[1][i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 2000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
            }
        }

        [Test]
        public void Sin1000Hz2000Hz_32bit()
        {
            var (data, sampleRate) = WaveFile.Read(Path.Combine("dataset", "sin1000hz2000hz_32bit.wav"));

            Assert.That(data.Length == 2);
            Assert.That(data[0].Count == 1600);
            Assert.That(data[1].Count == 1600);
            Assert.That(sampleRate == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[0][i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[1][i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 2000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }
        }
    }
}
