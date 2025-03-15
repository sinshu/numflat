using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.MultivariateAnalyses;

namespace NumFlatTest.MultivariateAnalysesTests
{
    public class ComplexLinearRegressionTests
    {
        [Test]
        public void WithoutNoise()
        {
            Vec<Complex> coefficients = [
                new Complex(1, 2),
                new Complex(3, 4),
                new Complex(5, 6)];
            var intercept = new Complex(7, 8);

            var random = new Random(42);
            var xs = new List<Vec<Complex>>();
            var ys = new List<Complex>();
            for (var i = 0; i < 100; i++)
            {
                var x = Enumerable.Range(0, 3).Select(k => new Complex(random.NextGaussian(), random.NextGaussian())).ToVector();
                var y = Vec.Dot(coefficients, x, true) + intercept;
                xs.Add(x);
                ys.Add(y);
            }

            var regression = xs.LinearRegression(ys);

            NumAssert.AreSame(coefficients, regression.Coefficients, 1.0E-6);
            Assert.That(intercept.Real, Is.EqualTo(regression.Intercept.Real).Within(1.0E-6));
            Assert.That(intercept.Imaginary, Is.EqualTo(regression.Intercept.Imaginary).Within(1.0E-6));
            foreach (var (x, y) in xs.Zip(ys))
            {
                var transformed = regression.Transform(x);
                Assert.That(transformed.Real, Is.EqualTo(y.Real).Within(1.0E-6));
                Assert.That(transformed.Imaginary, Is.EqualTo(y.Imaginary).Within(1.0E-6));
            }

            // The intercept is equal to the mean of ys if the regularization is very strong.
            var mean = ys.Average();
            var estimated = xs.LinearRegression(ys, 1000000).Intercept;
            Assert.That(mean.Real, Is.EqualTo(estimated.Real).Within(1.0E-3));
            Assert.That(mean.Imaginary, Is.EqualTo(estimated.Imaginary).Within(1.0E-3));
        }

        [Test]
        public void WithNoise()
        {
            Vec<Complex> coefficients = [
                new Complex(1, 2),
                new Complex(3, 4),
                new Complex(5, 6)];
            var intercept = new Complex(7, 8);

            var random = new Random(42);
            var xs = new List<Vec<Complex>>();
            var ys = new List<Complex>();
            for (var i = 0; i < 100; i++)
            {
                var x = Enumerable.Range(0, 3).Select(k => new Complex(random.NextGaussian(), random.NextGaussian())).ToVector();
                var noise = new Complex(random.NextGaussian(), random.NextGaussian()) / 5;
                var y = Vec.Dot(coefficients, x, true) + intercept + noise;
                xs.Add(x);
                ys.Add(y);
            }

            var regression = xs.LinearRegression(ys);

            NumAssert.AreSame(coefficients, regression.Coefficients, 0.1);
            Assert.That(intercept.Real, Is.EqualTo(regression.Intercept.Real).Within(0.1));
            Assert.That(intercept.Imaginary, Is.EqualTo(regression.Intercept.Imaginary).Within(0.1));
        }
    }
}
