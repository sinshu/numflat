using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MathLinqTests
{
    public class ScalarWeightedSecondOrderDouble
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

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void MeanAndVariance(int count, int ddof)
        {
            var random = new Random(42);
            var xs = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var weights = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = xs.MeanAndVariance(weights, ddof);
            var expectedMean = RefAverage(xs, weights);
            var expectedVariance = RefVariance(xs, weights, ddof);
            Assert.That(actual.Mean, Is.EqualTo(expectedMean).Within(1.0E-12));
            Assert.That(actual.Variance, Is.EqualTo(expectedVariance).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void MeanAndStandardDeviation(int count, int ddof)
        {
            var random = new Random(42);
            var xs = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var weights = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = xs.MeanAndStandardDeviation(weights, ddof);
            var expectedMean = RefAverage(xs, weights);
            var expectedStandardDeviation = RefStandardDeviation(xs, weights, ddof);
            Assert.That(actual.Mean, Is.EqualTo(expectedMean).Within(1.0E-12));
            Assert.That(actual.StandardDeviation, Is.EqualTo(expectedStandardDeviation).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void Variance(int count, int ddof)
        {
            var random = new Random(42);
            var xs = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var weights = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = xs.Variance(weights, ddof);
            var expected = RefVariance(xs, weights, ddof);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void StandardDeviation(int count, int ddof)
        {
            var random = new Random(42);
            var xs = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var weights = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = xs.StandardDeviation(weights, ddof);
            var expected = RefStandardDeviation(xs, weights, ddof);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void Covariance(int count, int ddof)
        {
            var random = new Random(42);
            var xs = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var ys = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();
            var weights = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = xs.Covariance(ys, weights, ddof);
            var expected = RefCovariance(xs, ys, weights, ddof);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        private static double RefAverage(IEnumerable<double> xs, IEnumerable<double> weights)
        {
            var xSum = 0.0;
            var wSum = 0.0;
            foreach (var (weight, x) in weights.Zip(xs))
            {
                xSum += weight * x;
                wSum += weight;
            }
            return xSum / wSum;
        }

        private static double RefVariance(IEnumerable<double> xs, IEnumerable<double> weights, int ddof)
        {
            var mean = RefAverage(xs, weights);
            var dSum = 0.0;
            var w1Sum = 0.0;
            var w2Sum = 0.0;
            foreach (var (weight, x) in weights.Zip(xs))
            {
                var d = x - mean;
                dSum += weight * d * d;
                w1Sum += weight;
                w2Sum += weight * weight;
            }
            var den = w1Sum - ddof * (w2Sum / w1Sum);
            return dSum / den;
        }

        private static double RefStandardDeviation(IEnumerable<double> xs, IEnumerable<double> weights, int ddof)
        {
            return Math.Sqrt(RefVariance(xs, weights, ddof));
        }

        private static double RefCovariance(IEnumerable<double> xs, IEnumerable<double> ys, IEnumerable<double> weights, int ddof)
        {
            var xMean = RefAverage(xs, weights);
            var yMean = RefAverage(ys, weights);
            var dSum = 0.0;
            var w1Sum = 0.0;
            var w2Sum = 0.0;
            foreach (var (weight, x, y) in weights.Zip(xs, ys))
            {
                dSum += weight * (x - xMean) * (y - yMean);
                w1Sum += weight;
                w2Sum += weight * weight;
            }
            var den = w1Sum - ddof * (w2Sum / w1Sum);
            return dSum / den;
        }
    }
}
