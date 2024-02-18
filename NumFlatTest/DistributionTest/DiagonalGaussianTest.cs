﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Distributions;

namespace NumFlatTest
{
    public class DiagonalGaussianTest
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
    }
}
