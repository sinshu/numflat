using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest.MathLinqTests
{
    public class ScalarSecondOrderComplex
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        public void Average(int count)
        {
            var random = new Random(42);
            var values = Enumerable.Range(0, count).Select(i => new Complex(random.NextDouble(), random.NextDouble())).ToArray();

            var actual = values.Average();
            var expected = new Complex(values.Select(x => x.Real).Average(), values.Select(x => x.Imaginary).Average());

            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(5, 0)]
        [TestCase(10, 1)]
        public void Variance(int count, int ddof)
        {
            var random = new Random(42);
            var values = Enumerable.Range(0, count).Select(i => new Complex(random.NextDouble(), random.NextDouble())).ToArray();

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
            var values = Enumerable.Range(0, count).Select(i => new Complex(random.NextDouble(), random.NextDouble())).ToArray();

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
            var values = Enumerable.Range(0, count).Select(i => new Complex(random.NextDouble(), random.NextDouble())).ToArray();

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
            var values = Enumerable.Range(0, count).Select(i => new Complex(random.NextDouble(), random.NextDouble())).ToArray();

            var actual = values.MeanAndStandardDeviation(ddof);

            var expectedMean = values.Average();
            var expectedStandardDeviation = MathNetStandardDeviation(values, ddof);

            Assert.That(actual.Mean.Real, Is.EqualTo(expectedMean.Real).Within(1.0E-12));
            Assert.That(actual.Mean.Imaginary, Is.EqualTo(expectedMean.Imaginary).Within(1.0E-12));
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
            var values1 = Enumerable.Range(0, count).Select(i => new Complex(random1.NextDouble(), random1.NextDouble())).ToArray();
            var random2 = new Random(57);
            var values2 = Enumerable.Range(0, count).Select(i => new Complex(random2.NextDouble(), random2.NextDouble())).ToArray();

            var actual = values1.Covariance(values2, ddof);
            var expected = MathNetCovariance(values1, values2, ddof);
            Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(1.0E-12));
            Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0E-12));
        }

        private static double MathNetVariance(IEnumerable<Complex> xs, int ddof)
        {
            var mean = xs.Average();
            var sum = 0.0;
            var count = 0;
            foreach (var x in xs)
            {
                var d = x - mean;
                sum += (d * d.Conjugate()).Real;
                count++;
            }
            return sum / (count - ddof);
        }

        private static double MathNetStandardDeviation(IEnumerable<Complex> xs, int ddof)
        {
            return Math.Sqrt(MathNetVariance(xs, ddof));
        }

        private static Complex MathNetCovariance(IEnumerable<Complex> xs, IEnumerable<Complex> ys, int ddof)
        {
            var xMean = xs.Average();
            var yMean = ys.Average();
            var sum = Complex.Zero;
            var count = 0;
            foreach (var (x, y) in xs.Zip(ys))
            {
                sum += (x - xMean) * (y - yMean).Conjugate();
                count++;
            }
            return sum / (count - ddof);
        }
    }
}
