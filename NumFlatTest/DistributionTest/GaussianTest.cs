using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Distributions;

namespace NumFlatTest
{
    public class GaussianTest
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
    }
}
