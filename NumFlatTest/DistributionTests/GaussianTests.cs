using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Distributions;

namespace NumFlatTest
{
    public class GaussianTests
    {
        [TestCase(0, 0, -3.56509784)]
        [TestCase(0, 1, -3.12759784)]
        [TestCase(2, 0, -4.06509784)]
        [TestCase(3, 7, -7.06509784)]
        public void LogPdf(int x, int y, double expected)
        {
            var mean = new double[] { 1, 2 }.ToVector();
            var cov = new double[,]
            {
                { 3, 1 },
                { 1, 3 },
            }
            .ToMatrix();

            Gaussian gaussian;
            double actual;
            using (mean.EnsureUnchanged())
            using (cov.EnsureUnchanged())
            {
                gaussian = new Gaussian(mean, cov);
                actual = gaussian.LogPdf(new double[] { x, y }.ToVector());
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
        }

        [TestCase(0, 0, 0, 2.20679652E-3)]
        [TestCase(-1, 5, 4, 2.46408200E-4)]
        [TestCase(3, -5, 1, 2.60729567E-8)]
        [TestCase(2, 3, -2, 5.23689335E-7)]
        public void Pdf(int x, int y, int z, double expected)
        {
            var mean = new double[] { 1, 2, 3 }.ToVector();
            var cov = new double[,]
            {
                { 3, 1, 2 },
                { 1, 3, 1 },
                { 2, 1, 3 },
            }
            .ToMatrix();

            Gaussian gaussian;
            double actual;
            using (mean.EnsureUnchanged())
            using (cov.EnsureUnchanged())
            {
                gaussian = new Gaussian(mean, cov);
                actual = gaussian.Pdf(new double[] { x, y, z }.ToVector());
            }

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-6));
        }

        [Test]
        public void ToGaussian()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var xs = data.Rows;

            var actual = xs.ToGaussian();

            var (mean, covariance) = xs.MeanAndCovariance();

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Covariance, covariance, 1.0E-12);
        }

        [Test]
        public void ToGaussian_Weighted()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var random = new Random(57);
            var xs = data.Rows;
            var weights = xs.Select(x => random.NextDouble() + 1).ToArray();

            var actual = xs.ToGaussian(weights);

            var (mean, covariance) = xs.MeanAndCovariance(weights);

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Covariance, covariance, 1.0E-12);
        }

        [Test]
        public void ToGaussian_Regularize()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var xs = data.Rows;
            var regularization = 0.3;

            var actual = xs.ToGaussian(regularization);

            var (mean, covariance) = xs.MeanAndCovariance();
            covariance += regularization * MatrixBuilder.Identity<double>(data.ColCount);

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Covariance, covariance, 1.0E-12);
        }

        [Test]
        public void ToGaussian_Weighted_Regularize()
        {
            var data = TestMatrix.RandomDouble(42, 10, 3, 10);
            var random = new Random(57);
            var xs = data.Rows;
            var weights = xs.Select(x => random.NextDouble() + 1).ToArray();
            var regularization = 0.3;

            var actual = xs.ToGaussian(weights, regularization);

            var (mean, covariance) = xs.MeanAndCovariance(weights);
            covariance += regularization * MatrixBuilder.Identity<double>(data.ColCount);

            NumAssert.AreSame(actual.Mean, mean, 1.0E-12);
            NumAssert.AreSame(actual.Covariance, covariance, 1.0E-12);
        }

        [Test]
        public void Mahalanobis()
        {
            var mean = new double[] { 1, 2, 3 }.ToVector();
            var cov = new double[,]
            {
                { 3, 1, 2 },
                { 1, 3, 1 },
                { 2, 1, 3 },
            }
            .ToMatrix();

            var gaussian = new Gaussian(mean, cov);
            var x = new double[] { 4, 5, 6 }.ToVector();

            var d = x - gaussian.Mean;
            var expected2 = d * (gaussian.Covariance.Inverse() * d);
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
        public void Bhattacharyya1()
        {
            var mean1 = new double[] { 1, 2, 3 }.ToVector();
            var cov1 = new double[,]
            {
                { 3.5, 1.0, 0.1 },
                { 1.0, 4.5, 0.9 },
                { 0.1, 0.9, 2.5 },
            }
            .ToMatrix();

            var mean2 = new double[] { 1.5, 2.4, 3.3 }.ToVector();
            var cov2 = new double[,]
            {
                { 4.5, 1.1, 0.3 },
                { 1.1, 3.5, 0.7 },
                { 0.3, 0.7, 2.6 },
            }
            .ToMatrix();

            var x = new Gaussian(mean1, cov1);
            var y = new Gaussian(mean2, cov2);

            using (mean1.EnsureUnchanged())
            using (cov1.EnsureUnchanged())
            using (mean2.EnsureUnchanged())
            using (cov2.EnsureUnchanged())
            {
                var expected = 0.02204707507666583;
                var actual = x.Bhattacharyya(y);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }

            Assert.That(x.Bhattacharyya(x), Is.EqualTo(0.0).Within(1.0E-12));
            Assert.That(y.Bhattacharyya(y), Is.EqualTo(0.0).Within(1.0E-12));
        }

        [Test]
        public void Bhattacharyya2()
        {
            var mean1 = new double[] { 1, 2, 3 }.ToVector();
            var variance1 = new double[] { 3.5, 4.5, 2.5 }.ToVector();

            var mean2 = new double[] { 1.5, 2.4, 3.3 }.ToVector();
            var variance2 = new double[] { 4.5, 3.5, 2.6 }.ToVector();

            var x = new Gaussian(mean1, variance1.ToDiagonalMatrix());
            var y = new Gaussian(mean2, variance2.ToDiagonalMatrix());

            using (mean1.EnsureUnchanged())
            using (variance1.EnsureUnchanged())
            using (mean2.EnsureUnchanged())
            using (variance2.EnsureUnchanged())
            {
                var expected = 0.025194578549721295;
                var actual = x.Bhattacharyya(y);
                Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
            }
        }
    }
}
