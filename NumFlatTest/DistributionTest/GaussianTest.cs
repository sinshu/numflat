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
    }
}
