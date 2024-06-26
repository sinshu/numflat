﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MathLinqTests
{
    public class ScalarSecondOrderTestsDouble
    {
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void Variance(int count, int ddof)
        {
            var random = new Random(42);
            var values = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = values.Variance(ddof);
            var expected = MathNetVariance(values, ddof);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void MeanAndVariance(int count, int ddof)
        {
            var random = new Random(42);
            var values = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = values.MeanAndVariance(ddof);

            var expectedMean = values.Average();
            var expectedVariance = MathNetVariance(values, ddof);

            Assert.That(actual.Mean, Is.EqualTo(expectedMean).Within(1.0E-12));
            Assert.That(actual.Variance, Is.EqualTo(expectedVariance).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void StandardDeviation(int count, int ddof)
        {
            var random = new Random(42);
            var values = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = values.StandardDeviation(ddof);
            var expected = MathNetStandardDeviation(values, ddof);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void MeanAndStandardDeviation(int count, int ddof)
        {
            var random = new Random(42);
            var values = Enumerable.Range(0, count).Select(i => random.NextDouble()).ToArray();

            var actual = values.MeanAndStandardDeviation(ddof);

            var expectedMean = values.Average();
            var expectedStandardDeviation = MathNetStandardDeviation(values, ddof);

            Assert.That(actual.Mean, Is.EqualTo(expectedMean).Within(1.0E-12));
            Assert.That(actual.StandardDeviation, Is.EqualTo(expectedStandardDeviation).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void Covariance(int count, int ddof)
        {
            var random1 = new Random(42);
            var values1 = Enumerable.Range(0, count).Select(i => random1.NextDouble()).ToArray();
            var random2 = new Random(57);
            var values2 = Enumerable.Range(0, count).Select(i => random2.NextDouble()).ToArray();

            var actual = values1.Covariance(values2, ddof);
            var expected = MathNetCovariance(values1, values2, ddof);
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        private static double MathNetVariance(IEnumerable<double> xs, int ddof)
        {
            if (ddof == 0)
            {
                return MathNet.Numerics.Statistics.Statistics.PopulationVariance(xs);
            }
            else if (ddof == 1)
            {
                return MathNet.Numerics.Statistics.Statistics.Variance(xs);
            }
            else
            {
                throw new Exception();
            }
        }

        private static double MathNetStandardDeviation(IEnumerable<double> xs, int ddof)
        {
            if (ddof == 0)
            {
                return MathNet.Numerics.Statistics.Statistics.PopulationStandardDeviation(xs);
            }
            else if (ddof == 1)
            {
                return MathNet.Numerics.Statistics.Statistics.StandardDeviation(xs);
            }
            else
            {
                throw new Exception();
            }
        }

        private static double MathNetCovariance(IEnumerable<double> xs, IEnumerable<double> ys, int ddof)
        {
            if (ddof == 0)
            {
                return MathNet.Numerics.Statistics.Statistics.PopulationCovariance(xs, ys);
            }
            else if (ddof == 1)
            {
                return MathNet.Numerics.Statistics.Statistics.Covariance(xs, ys);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
