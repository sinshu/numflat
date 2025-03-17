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
                var y = coefficients * x + intercept;
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
                var noise = new Complex(random.NextGaussian(), random.NextGaussian()) / 3;
                var y = coefficients * x + intercept + noise;
                xs.Add(x);
                ys.Add(y);
            }

            var regression = xs.LinearRegression(ys);

            NumAssert.AreSame(coefficients, regression.Coefficients, 0.1);
            Assert.That(intercept.Real, Is.EqualTo(regression.Intercept.Real).Within(0.1));
            Assert.That(intercept.Imaginary, Is.EqualTo(regression.Intercept.Imaginary).Within(0.1));
        }

        [Test]
        public void Gpt45()
        {
            var path = Path.Combine("dataset", "lr_gpt45.csv");
            var xs = new List<Vec<Complex>>();
            var ys = new List<Complex>();
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var values = line.Split(',').Select(value => new Complex(double.Parse(value), 0));
                xs.Add(values.Take(4).ToVector());
                ys.Add(values.Last());
            }

            var regression = xs.LinearRegression(ys);

            Vec<Complex> expectedCoefficients = [3.5108, -1.9850, 4.1760, 1.6952];
            NumAssert.AreSame(expectedCoefficients, regression.Coefficients, 1.0E-3);

            var expectedIntercept = new Complex(4.9926, 0);
            Assert.That(expectedIntercept.Real, Is.EqualTo(regression.Intercept.Real).Within(1.0E-3));
            Assert.That(expectedIntercept.Imaginary, Is.EqualTo(regression.Intercept.Imaginary).Within(1.0E-3));
        }
    }
}
