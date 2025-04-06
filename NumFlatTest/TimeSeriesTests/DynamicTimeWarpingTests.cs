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
        [TestCase("a", "b", 1)]
        [TestCase("abc", "def", 3)]
        [TestCase("abcdef", "azced", 3)]
        [TestCase("kitten", "sitting", 3)]
        [TestCase("intention", "execution", 5)]
        [TestCase("Saturday", "Sunday", 3)]
        [TestCase("distance", "editing", 5)]
        [TestCase("abcdefg", "acdfg", 2)]
        public void Compute(string x, string y, double expected)
        {
            var distance = DynamicTimeWarping.GetDistance(x.ToArray(), y.ToArray(), new Levenshtein());
            Assert.That(distance, Is.EqualTo(expected));
        }
    }

    class Levenshtein : IDistance<char, char>
    {
        public double GetDistance(char x, char y)
        {
            return x == y ? 0 : 1;
        }
    }
}
