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
    public class LinearRegressionTests
    {
        [Test]
        public void WithoutNoise()
        {
            Vec<double> coefficients = [1, 2, 3];
            var intercept = 4.0;

            var random = new Random(42);
            var xs = new List<Vec<double>>();
            var ys = new List<double>();
            for (var i = 0; i < 100; i++)
            {
                var x = Enumerable.Range(0, 3).Select(k => random.NextGaussian()).ToVector();
                var y = coefficients * x + intercept;
                xs.Add(x);
                ys.Add(y);
            }

            var regression = xs.LinearRegression(ys);

            NumAssert.AreSame(coefficients, regression.Coefficients, 1.0E-6);
            Assert.That(intercept, Is.EqualTo(regression.Intercept).Within(1.0E-6));
            foreach (var (x, y) in xs.Zip(ys))
            {
                Assert.That(regression.Transform(x), Is.EqualTo(y).Within(1.0E-6));
            }

            // The intercept is equal to the mean of ys if the regularization is very strong.
            var mean = ys.Average();
            var estimated = xs.LinearRegression(ys, 1000000).Intercept;
            Assert.That(mean, Is.EqualTo(estimated).Within(1.0E-3));
        }

        [Test]
        public void WithNoise()
        {
            Vec<double> coefficients = [1, 2, 3];
            var intercept = 4.0;

            var random = new Random(42);
            var xs = new List<Vec<double>>();
            var ys = new List<double>();
            for (var i = 0; i < 100; i++)
            {
                var x = Enumerable.Range(0, 3).Select(k => random.NextGaussian()).ToVector();
                var noise = random.NextGaussian() / 3;
                var y = coefficients * x + intercept + noise;
                xs.Add(x);
                ys.Add(y);
            }

            var regression = xs.LinearRegression(ys);

            NumAssert.AreSame(coefficients, regression.Coefficients, 0.1);
            Assert.That(intercept, Is.EqualTo(regression.Intercept).Within(0.1));
        }
    }
}
