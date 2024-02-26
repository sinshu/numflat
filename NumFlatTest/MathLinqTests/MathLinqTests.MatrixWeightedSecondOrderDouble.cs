using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MathLinqTests_MatrixWeightedSecondOrderDouble
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

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            MathLinq.Mean(data.Select(x => x.ToArray().ToMatrix()), weights, actual);

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

        private static Mat<double> RefMean(IEnumerable<Mat<double>> xs, IEnumerable<double> weights)
        {
            var first = xs.First();
            var dst = new Mat<double>(first.RowCount, first.ColCount);
            for (var row = 0; row < dst.RowCount; row++)
            {
                for (var col = 0; col < dst.ColCount; col++)
                {
                    dst[row, col] = xs.Select(x => x[row, col]).Average(weights);
                }
            }
            return dst;
        }

        private static Mat<double>[] CreateData(int seed, int rowCount, int colCount, int matCount)
        {
            var random = new Random(seed);

            var data = new List<Mat<double>>();

            for (var i = 0; i < matCount; i++)
            {
                var x = new Mat<double>(rowCount, colCount);
                for (var row = 0; row < rowCount; row++)
                {
                    for (var col = 0; col < colCount; col++)
                    {
                        x[row, col] = random.NextDouble();
                    }
                }
                data.Add(x);
            }

            return data.ToArray();
        }
    }
}
