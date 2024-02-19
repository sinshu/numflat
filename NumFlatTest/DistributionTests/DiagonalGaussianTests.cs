using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Distributions;

namespace NumFlatTest
{
    public class DiagonalGaussianTests
    {
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(2, 0)]
        [TestCase(3, 7)]
        public void LogPdf(int x, int y)
        {
            var mean = new double[] { 1, 2 }.ToVector();
            var variance = new double[] { 2, 3 }.ToVector();

            DiagonalGaussian diagonal;
            double actual;
            using (mean.EnsureUnchanged())
            using (variance.EnsureUnchanged())
            {
                diagonal = new DiagonalGaussian(mean, variance);
                actual = diagonal.LogPdf(new double[] { x, y }.ToVector());
            }

            var cov = variance.ToDiagonalMatrix();
            var gaussian = new Gaussian(mean, cov);
            var expected = gaussian.LogPdf(new double[] { x, y }.ToVector());

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, -1, 2)]
        [TestCase(-2, 3, 1)]
        [TestCase(3, 0, -1)]
        public void LogPdf(int x, int y, int z)
        {
            var mean = new double[] { 1, 2, 3 }.ToVector();
            var variance = new double[] { 2, 3, 5 }.ToVector();

            DiagonalGaussian diagonal;
            double actual;
            using (mean.EnsureUnchanged())
            using (variance.EnsureUnchanged())
            {
                diagonal = new DiagonalGaussian(mean, variance);
                actual = diagonal.LogPdf(new double[] { x, y, z }.ToVector());
            }

            var cov = variance.ToDiagonalMatrix();
            var gaussian = new Gaussian(mean, cov);
            var expected = gaussian.LogPdf(new double[] { x, y, z }.ToVector());

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [Test]
        public void ToDiagonalGaussian()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var xs = data.Rows;

            var actual = xs.ToDiagonalGaussian();

            var (mean, variance) = xs.MeanAndVariance();

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Variance, variance, 1.0E-12);
        }

        [Test]
        public void ToDiagonalGaussian_Weighted()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var random = new Random(57);
            var xs = data.Rows;
            var weights = xs.Select(x => random.NextDouble() + 1).ToArray();

            var actual = xs.ToDiagonalGaussian(weights);

            var (mean, variance) = xs.MeanAndVariance(weights);

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Variance, variance, 1.0E-12);
        }

        [Test]
        public void ToDiagonalGaussian_Regularize()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var xs = data.Rows;
            var regularization = 0.3;

            var actual = xs.ToDiagonalGaussian(regularization);

            var (mean, variance) = xs.MeanAndVariance();
            variance += regularization * VectorBuilder.Fill(data.ColCount, 1.0);

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Variance, variance, 1.0E-12);
        }

        [Test]
        public void ToDiagonalGaussian_Weighted_Regularize()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var random = new Random(57);
            var xs = data.Rows;
            var weights = xs.Select(x => random.NextDouble() + 1).ToArray();
            var regularization = 0.3;

            var actual = xs.ToDiagonalGaussian(weights, regularization);

            var (mean, variance) = xs.MeanAndVariance(weights);
            variance += regularization * VectorBuilder.Fill(data.ColCount, 1.0);

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Variance, variance, 1.0E-12);
        }

        [Test]
        public void Mahalanobis()
        {
            var mean = new double[] { 1, 2, 3 }.ToVector();
            var variance = new double[] { 2, 2.5, 3 }.ToVector();
            var cov = variance.ToDiagonalMatrix();

            var gaussian = new DiagonalGaussian(mean, variance);
            var x = new double[] { 4, 5, 6 }.ToVector();

            var d = x - gaussian.Mean;
            var expected2 = d * (cov.Inverse() * d);
            var expected1 = Math.Sqrt(expected2);

            double actual1;
            double actual2;
            using (x.EnsureUnchanged())
            {
                actual1 = gaussian.Mahalanobis(x);
                actual2 = gaussian.MahalanobisSquared(x);
            }

            Assert.That(actual1, Is.EqualTo(expected1).Within(1.0E-12));
            Assert.That(actual2, Is.EqualTo(expected2).Within(1.0E-12));
        }

        [Test]
        public void Bhattacharyya()
        {
            var mean1 = new double[] { 1, 2, 3 }.ToVector();
            var variance1 = new double[] { 3.5, 4.5, 2.5 }.ToVector();

            var mean2 = new double[] { 1.5, 2.4, 3.3 }.ToVector();
            var variance2 = new double[] { 4.5, 3.5, 2.6 }.ToVector();

            var x = new DiagonalGaussian(mean1, variance1);
            var y = new DiagonalGaussian(mean2, variance2);

            using (mean1.EnsureUnchanged())
            using (variance1.EnsureUnchanged())
            using (mean2.EnsureUnchanged())
            using (variance2.EnsureUnchanged())
            {
                var expected = 0.025194578549721295;
                var actual = x.Bhattacharyya(y);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }

            Assert.That(x.Bhattacharyya(x), Is.EqualTo(0.0).Within(1.0E-12));
            Assert.That(y.Bhattacharyya(y), Is.EqualTo(0.0).Within(1.0E-12));
        }
    }
}
