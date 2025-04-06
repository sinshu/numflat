using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.TimeSeries;

namespace NumFlatTest.TimeSeriesTests
{
    public class DynamicTimeWarpingTests
    {
        [Test]
        public void Zero()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var distance = DynamicTimeWarping.GetDistance(xs, ys, new IntAbs());
                Assert.That(distance, Is.EqualTo(0));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(ys, xs, new IntAbs());
                Assert.That(distance, Is.EqualTo(0));
            }
        }

        [Test]
        public void One()
        {
            int[] xs = [1, 1, 1, 2, 2, 2, 3, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 3];
            int[] ys = [1, 1, 1, 1, 2, 2, 3, 3, 4, 3, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3];

            {
                var distance = DynamicTimeWarping.GetDistance(xs, ys, new IntAbs());
                Assert.That(distance, Is.EqualTo(1));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(ys, xs, new IntAbs());
                Assert.That(distance, Is.EqualTo(1));
            }
        }

        [TestCase("a", "b", 1)]
        [TestCase("abc", "def", 3)]
        [TestCase("abcdef", "azced", 3)]
        [TestCase("kitten", "sitting", 3)]
        [TestCase("intention", "execution", 5)]
        [TestCase("Saturday", "Sunday", 3)]
        [TestCase("distance", "editing", 5)]
        [TestCase("abcdefg", "acdfg", 2)]
        public void Levenshtein(string x, string y, double expected)
        {
            {
                var distance = DynamicTimeWarping.GetDistance(x.ToArray(), y.ToArray(), new Levenshtein());
                Assert.That(distance, Is.EqualTo(expected));
            }

            {
                var distance = DynamicTimeWarping.GetDistance(y.ToArray(), x.ToArray(), new Levenshtein());
                Assert.That(distance, Is.EqualTo(expected));
            }
        }
    }

    class Levenshtein : IDistance<char, char>
    {
        public double GetDistance(char x, char y)
        {
            return x == y ? 0 : 1;
        }
    }

    class IntAbs : IDistance<int, int>
    {
        public double GetDistance(int x, int y)
        {
            return Math.Abs(x - y);
        }
    }
}
