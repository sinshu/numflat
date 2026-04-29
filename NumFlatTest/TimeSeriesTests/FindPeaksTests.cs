using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.TimeSeries;

namespace NumFlatTest.TimeSeriesTests
{
    public class FindPeaksTests
    {
        [Test]
        public void Default()
        {
            var path = Path.Combine("dataset", "find_peaks_default.csv");
            var referenceData = File
                .ReadLines(path)
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(value => (double.Parse(value[0]), int.Parse(value[1])))
                .ToArray();

            var x = referenceData.Select(pair => pair.Item1).ToVector();

            var expected = new List<(int Index, double Value)>();
            for (var i = 0; i < x.Count; i++)
            {
                if (referenceData[i].Item2 == 1)
                {
                    expected.Add((i, x[i]));
                }
            }

            var actual = x.FindPeaks();

            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            foreach (var (a, b) in actual.Zip(expected))
            {
                Assert.That(a.Index, Is.EqualTo(b.Index));
                Assert.That(a.Value, Is.EqualTo(b.Value));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        public void DistanceConstraint(int distance)
        {
            var path = Path.Combine("dataset", "find_peaks_distance.csv");
            var lines = File.ReadLines(path).ToArray();
            var header = lines[0].Split(',');
            var distanceColumn = Array.IndexOf(header, $"distance_{distance}");

            var referenceData = lines
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(value => (double.Parse(value[0]), int.Parse(value[distanceColumn])))
                .ToArray();

            var x = referenceData.Select(pair => pair.Item1).ToVector();

            var expected = new List<(int Index, double Value)>();
            for (var i = 0; i < x.Count; i++)
            {
                if (referenceData[i].Item2 == 1)
                {
                    expected.Add((i, x[i]));
                }
            }

            var actual = x.FindPeaksWithDistanceConstraint(distance);

            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            foreach (var (a, b) in actual.Zip(expected))
            {
                Assert.That(a.Index, Is.EqualTo(b.Index));
                Assert.That(a.Value, Is.EqualTo(b.Value));
            }
        }

        [TestCase(0.05)]
        [TestCase(0.1)]
        [TestCase(0.2)]
        [TestCase(0.4)]
        [TestCase(0.8)]
        public void ProminenceConstraint(double prominence)
        {
            var path = Path.Combine("dataset", "find_peaks_prominence.csv");
            var lines = File.ReadLines(path).ToArray();
            var header = lines[0].Split(',');
            var prominenceColumn = Array.IndexOf(header, $"prominence_{prominence}");

            var referenceData = lines
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(value => (double.Parse(value[0]), int.Parse(value[prominenceColumn])))
                .ToArray();

            var x = referenceData.Select(pair => pair.Item1).ToVector();

            var expected = new List<(int Index, double Value)>();
            for (var i = 0; i < x.Count; i++)
            {
                if (referenceData[i].Item2 == 1)
                {
                    expected.Add((i, x[i]));
                }
            }

            var actual = x.FindPeaksWithProminenceConstraint(prominence);

            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            foreach (var (a, b) in actual.Zip(expected))
            {
                Assert.That(a.Index, Is.EqualTo(b.Index));
                Assert.That(a.Value, Is.EqualTo(b.Value));
            }
        }
    }
}
