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
        public void Read_Sin1000Hz_16bit()
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
        public void Read_Sin1000Hz_32bit()
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
        public void Read_Sin1000Hz2000Hz_16bit()
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
        public void Read_Sin1000Hz2000Hz_32bit()
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

        [Test]
        public void ReadMono_Sin1000Hz_16bit()
        {
            var (data, sampleRate) = WaveFile.ReadMono(Path.Combine("dataset", "sin1000hz_16bit.wav"));

            Assert.That(data.Count == 1600);
            Assert.That(sampleRate == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
            }
        }

        [Test]
        public void ReadMono_Sin1000Hz_32bit()
        {
            var (data, sampleRate) = WaveFile.ReadMono(Path.Combine("dataset", "sin1000hz_32bit.wav"));

            Assert.That(data.Count == 1600);
            Assert.That(sampleRate == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data[i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }
        }

        [Test]
        public void ReadMono_Sin1000Hz2000Hz_16bit()
        {
            var (data1, sampleRate1) = WaveFile.ReadMono(Path.Combine("dataset", "sin1000hz2000hz_16bit.wav"), 0);
            var (data2, sampleRate2) = WaveFile.ReadMono(Path.Combine("dataset", "sin1000hz2000hz_16bit.wav"), 1);

            Assert.That(data1.Count == 1600);
            Assert.That(data2.Count == 1600);
            Assert.That(sampleRate1 == 16000);
            Assert.That(sampleRate2 == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data1[i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate1 * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
            }

            for (var i = 0; i < 1600; i++)
            {
                var actual = data2[i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate2 * 2000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
            }
        }

        [Test]
        public void ReadMono_Sin1000Hz2000Hz_32bit()
        {
            var (data1, sampleRate1) = WaveFile.ReadMono(Path.Combine("dataset", "sin1000hz2000hz_32bit.wav"), 0);
            var (data2, sampleRate2) = WaveFile.ReadMono(Path.Combine("dataset", "sin1000hz2000hz_32bit.wav"), 1);

            Assert.That(data1.Count == 1600);
            Assert.That(data2.Count == 1600);
            Assert.That(sampleRate1 == 16000);
            Assert.That(sampleRate2 == 16000);

            for (var i = 0; i < 1600; i++)
            {
                var actual = data1[i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate1 * 1000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }

            for (var i = 0; i < 1600; i++)
            {
                var actual = data2[i];
                var expected = 0.5 * Math.Sin(2 * Math.PI * i / sampleRate2 * 2000);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Write(int channelCount)
        {
            {
                var random = new Random(42);
                var data = new Vec<double>[channelCount];
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = new Vec<double>(1600);
                    for (var t = 0; t < data[i].Count; t++)
                    {
                        data[i][t] = random.NextDouble() - 0.5;
                    }
                }
                WaveFile.Write("test.wav", data, 16000);
            }

            {
                var random = new Random(42);
                var (data, sampleRate) = WaveFile.Read("test.wav");
                Assert.That(data.Length == channelCount);
                Assert.That(sampleRate == 16000);
                for (var i = 0; i < data.Length; i++)
                {
                    Assert.That(data[i].Count == 1600);
                    for (var t = 0; t < 1600; t++)
                    {
                        var actual = data[i][t];
                        var expected = random.NextDouble() - 0.5;
                        Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
                    }
                }
            }
        }

        [Test]
        public void Write()
        {
            {
                var random = new Random(42);
                var data = new Vec<double>(1600);
                for (var t = 0; t < data.Count; t++)
                {
                    data[t] = random.NextDouble() - 0.5;
                }
                WaveFile.Write("test.wav", data, 16000);
            }

            {
                var random = new Random(42);
                var (data, sampleRate) = WaveFile.Read("test.wav");
                Assert.That(data.Length == 1);
                Assert.That(sampleRate == 16000);
                Assert.That(data[0].Count == 1600);
                for (var t = 0; t < 1600; t++)
                {
                    var actual = data[0][t];
                    var expected = random.NextDouble() - 0.5;
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-4));
                }
            }
        }
    }
}
