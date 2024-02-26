using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MathLinqTests_MatrixWeightedSecondOrderComplex
    {
        [TestCase(1, 1, 1, 2)]
        [TestCase(1, 1, 3, 2)]
        [TestCase(3, 2, 1, 4)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(4, 5, 6, 5)]
        [TestCase(3, 6, 4, 4)]
        public void Mean(int rowCount, int colCount, int matCount, int dstStride)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();
            var expected = RefMean(data, weights);

            var actual = TestMatrix.RandomComplex(0, rowCount, colCount, dstStride);
            MathLinq.Mean(data, weights, actual);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(3, 2, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(4, 5, 6)]
        [TestCase(3, 6, 4)]
        public void Mean_ExtensionMethod(int rowCount, int colCount, int matCount)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();
            var expected = RefMean(data, weights);

            var actual = data.Mean(weights);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 3, 0)]
        [TestCase(1, 1, 2, 3, 1)]
        [TestCase(2, 2, 1, 3, 0)]
        [TestCase(2, 2, 2, 3, 1)]
        [TestCase(3, 4, 3, 5, 0)]
        [TestCase(3, 2, 5, 4, 1)]
        public void Variance(int rowCount, int colCount, int matCount, int dstStride, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();
            var expected = RefVariance(data, weights, ddof);

            var mean = data.Mean(weights);
            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            MathLinq.Variance(data, weights, mean, actual, ddof);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 2, 1, 0)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(3, 4, 3, 0)]
        [TestCase(3, 2, 5, 1)]
        public void MeanAndVariance(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();

            var expectedMean = RefMean(data, weights);
            var expectedVariance = RefVariance(data, weights, ddof);

            var actual = data.MeanAndVariance(weights, ddof);

            NumAssert.AreSame(expectedMean, actual.Mean, 1.0E-12);
            NumAssert.AreSame(expectedVariance, actual.Variance, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 2, 1, 0)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(3, 4, 3, 0)]
        [TestCase(3, 2, 5, 1)]
        public void MeanAndStandardDeviation(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();

            var expectedMean = RefMean(data, weights);
            var expectedStandardDeviation = RefStandardDeviation(data, weights, ddof);

            var actual = data.MeanAndStandardDeviation(weights, ddof);

            NumAssert.AreSame(expectedMean, actual.Mean, 1.0E-12);
            NumAssert.AreSame(expectedStandardDeviation, actual.StandardDeviation, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 2, 1, 0)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(3, 4, 3, 0)]
        [TestCase(3, 2, 5, 1)]
        public void Variance_ExtensionMethod(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();

            var expected = RefVariance(data, weights, ddof);
            var actual = data.Variance(weights, ddof);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 2, 1, 0)]
        [TestCase(2, 2, 2, 1)]
        [TestCase(3, 4, 3, 0)]
        [TestCase(3, 2, 5, 1)]
        public void StandardDeviation_ExtensionMethod(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var random = new Random(57);
            var weights = data.Select(x => random.NextDouble()).ToArray();

            var expected = RefStandardDeviation(data, weights, ddof);
            var actual = data.StandardDeviation(weights, ddof);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static Mat<Complex> RefMean(IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights)
        {
            var first = xs.First();
            var dst = new Mat<Complex>(first.RowCount, first.ColCount);
            for (var row = 0; row < dst.RowCount; row++)
            {
                for (var col = 0; col < dst.ColCount; col++)
                {
                    dst[row, col] = xs.Select(x => x[row, col]).Average(weights);
                }
            }
            return dst;
        }

        private static Mat<double> RefVariance(IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, int ddof)
        {
            var first = xs.First();
            var dst = new Mat<double>(first.RowCount, first.ColCount);
            for (var row = 0; row < dst.RowCount; row++)
            {
                for (var col = 0; col < dst.ColCount; col++)
                {
                    dst[row, col] = xs.Select(x => x[row, col]).Variance(weights, ddof);
                }
            }
            return dst;
        }

        private static Mat<double> RefStandardDeviation(IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, int ddof)
        {
            var first = xs.First();
            var dst = new Mat<double>(first.RowCount, first.ColCount);
            for (var row = 0; row < dst.RowCount; row++)
            {
                for (var col = 0; col < dst.ColCount; col++)
                {
                    dst[row, col] = xs.Select(x => x[row, col]).StandardDeviation(weights, ddof);
                }
            }
            return dst;
        }

        private static Mat<Complex>[] CreateData(int seed, int rowCount, int colCount, int matCount)
        {
            var random = new Random(seed);

            var data = new List<Mat<Complex>>();

            for (var i = 0; i < matCount; i++)
            {
                var x = new Mat<Complex>(rowCount, colCount);
                for (var row = 0; row < rowCount; row++)
                {
                    for (var col = 0; col < colCount; col++)
                    {
                        x[row, col] = new Complex(random.NextDouble(), random.NextDouble());
                    }
                }
                data.Add(x);
            }

            return data.ToArray();
        }
    }
}
