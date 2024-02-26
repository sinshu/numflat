using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MathLinqTests_ScalarWeightedSecondOrderDouble
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        public void Average(int count)
        {
            var random = new Random(42);
            var xs = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var weights = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = xs.Average(weights);
            var expected = RefAverage(xs, weights);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        private static double RefAverage(IEnumerable<double> xs, IEnumerable<double> weights)
        {
            var xSum = 0.0;
            var wSUm = 0.0;
            foreach (var (weight, x) in weights.Zip(xs))
            {
                xSum += weight * x;
                wSUm += weight;
            }
            return xSum / wSUm;
        }
    }
}
