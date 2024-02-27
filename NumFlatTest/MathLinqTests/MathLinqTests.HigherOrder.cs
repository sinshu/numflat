using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MathLinqTests_HigherOrder
    {
        [TestCase(2, false)]
        [TestCase(3, false)]
        [TestCase(3, true)]
        [TestCase(5, false)]
        [TestCase(5, true)]
        public void Skewness(int count, bool unbiased)
        {
            var xs = TestVector.RandomDouble(42, count, 1);

            var actual = xs.Skewness(unbiased);

            var expected = unbiased
                ? MathNet.Numerics.Statistics.Statistics.Skewness(xs)
                : MathNet.Numerics.Statistics.Statistics.PopulationSkewness(xs);

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(3, false)]
        [TestCase(4, false)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        [TestCase(5, true)]
        public void Kurtosis(int count, bool unbiased)
        {
            var xs = TestVector.RandomDouble(42, count, 1);

            var actual = xs.Kurtosis(unbiased);

            var expected = unbiased
                ? MathNet.Numerics.Statistics.Statistics.Kurtosis(xs)
                : MathNet.Numerics.Statistics.Statistics.PopulationKurtosis(xs);

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
