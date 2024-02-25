using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MathLinqTests_ScalarSecondOrderDouble
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

            var actual = values.Variance(values.Average(), ddof);
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
    }
}
