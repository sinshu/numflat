using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MathLinq_DoubleTest
    {
        [TestCase(3, 1, 1)]
        [TestCase(3, 1, 4)]
        [TestCase(3, 10, 1)]
        [TestCase(3, 10, 2)]
        [TestCase(5, 20, 3)]
        public void Mean(int dim, int count, int dstStride)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetMean(data);

            var actual = Utilities.CreateRandomVectorDouble(0, dim, dstStride);
            data.Select(x => x.ToVector()).Mean(actual);

            for (var i = 0; i < dim; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }

            Utilities.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(3, 1)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void Mean_ExtensionMethod(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetMean(data);
            var actual = data.Select(x => x.ToVector()).Mean();

            for (var i = 0; i < dim; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]).Within(1.0E-12));
            }
        }

        [TestCase(3, 1, 0, 1, 3)]
        [TestCase(3, 1, 0, 2, 4)]
        [TestCase(3, 10, 1, 1, 3)]
        [TestCase(3, 10, 1, 4, 7)]
        [TestCase(5, 20, 2, 1, 5)]
        [TestCase(5, 20, 2, 5, 6)]
        public void Covariance(int dim, int count, int ddot, int meanStride, int covStride)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetCov(data, ddot);

            var mean = Utilities.CreateRandomVectorDouble(0, dim, meanStride);
            data.Select(x => x.ToVector()).Mean(mean);
            var actual = Utilities.CreateRandomMatrixDouble(0, dim, dim, covStride);
            data.Select(x => x.ToVector()).Covariance(mean, actual, ddot);

            for (var row = 0; row < dim; row++)
            {
                for (var col = 0; col < dim; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(mean);
            Utilities.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 2)]
        public void Covariance_ExtensionMethod_OneArg(int dim, int count, int ddot)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetCov(data, ddot);

            var actual = data.Select(x => x.ToVector()).Covariance(ddot);

            for (var row = 0; row < dim; row++)
            {
                for (var col = 0; col < dim; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
                }
            }
        }

        [TestCase(3, 2)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void Covariance_ExtensionMethod_NoArg(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var expected = MathNetCov(data, 1);

            var actual = data.Select(x => x.ToVector()).Covariance();

            for (var row = 0; row < dim; row++)
            {
                for (var col = 0; col < dim; col++)
                {
                    Assert.That(actual[row, col], Is.EqualTo(expected[row, col]).Within(1.0E-12));
                }
            }
        }

        [TestCase(3, 1, 0)]
        [TestCase(3, 10, 1)]
        [TestCase(5, 20, 2)]
        public void MeanAndCovariance_OneArg(int dim, int count, int ddot)
        {
            var data = CreateData(42, dim, count);
            var expectedMean = MathNetMean(data);
            var expectedCov = MathNetCov(data, ddot);

            var result = data.Select(x => x.ToVector()).MeanAndCovariance(ddot);

            for (var i = 0; i < dim; i++)
            {
                Assert.That(result.Mean[i], Is.EqualTo(expectedMean[i]).Within(1.0E-12));
            }

            for (var row = 0; row < dim; row++)
            {
                for (var col = 0; col < dim; col++)
                {
                    Assert.That(result.Covariance[row, col], Is.EqualTo(expectedCov[row, col]).Within(1.0E-12));
                }
            }
        }

        [TestCase(3, 2)]
        [TestCase(3, 10)]
        [TestCase(5, 20)]
        public void MeanAndCovariance_NoArg(int dim, int count)
        {
            var data = CreateData(42, dim, count);
            var expectedMean = MathNetMean(data);
            var expectedCov = MathNetCov(data, 1);

            var result = data.Select(x => x.ToVector()).MeanAndCovariance();

            for (var i = 0; i < dim; i++)
            {
                Assert.That(result.Mean[i], Is.EqualTo(expectedMean[i]).Within(1.0E-12));
            }

            for (var row = 0; row < dim; row++)
            {
                for (var col = 0; col < dim; col++)
                {
                    Assert.That(result.Covariance[row, col], Is.EqualTo(expectedCov[row, col]).Within(1.0E-12));
                }
            }
        }

        private static Vector<double> MathNetMean(IEnumerable<Vector<double>> xs)
        {
            Vector<double> sum = new DenseVector(xs.First().Count);
            var count = 0;
            foreach (var x in xs)
            {
                sum += x;
                count++;
            }
            return sum / count;
        }

        private static Matrix<double> MathNetCov(IEnumerable<Vector<double>> xs, int ddot)
        {
            var mean = MathNetMean(xs);

            Matrix<double> sum = new DenseMatrix(xs.First().Count);
            var count = 0;
            foreach (var x in xs)
            {
                var d = x - mean;
                sum += d.OuterProduct(d);
                count++;
            }
            return sum / (count - ddot);
        }

        private static Vector<double>[] CreateData(int seed, int dim, int count)
        {
            var random = new Random(seed);

            var data = new List<Vector<double>>();

            for (var i = 0; i < count; i++)
            {
                var elements = Enumerable.Range(0, dim).Select(i => random.NextDouble()).ToArray();
                var x = new DenseVector(elements);
                data.Add(x);
            }

            return data.ToArray();
        }
    }
}
