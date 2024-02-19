using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using NumFlat;

using MVec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using MMat = MathNet.Numerics.LinearAlgebra.Matrix<double>;

namespace NumFlatTest
{
    public class MathLinqTests_MatrixSecondOrderDouble
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
            var expected = MathNetMean(data);

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            MathLinq.Mean(data.Select(x => x.ToArray().ToMatrix()), actual);

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
            var expected = MathNetMean(data);
            var actual = data.Select(x => x.ToArray().ToMatrix()).Mean();
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0, 2, 2)]
        [TestCase(1, 1, 2, 1, 2, 2)]
        [TestCase(2, 3, 3, 1, 4, 4)]
        [TestCase(4, 3, 5, 0, 6, 6)]
        public void Variance(int rowCount, int colCount, int matCount, int ddof, int meanStride, int varStride)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expected = MathNetVar(data, ddof);

            var mean = TestMatrix.RandomDouble(0, rowCount, colCount, meanStride);
            MathLinq.Mean(data.Select(x => x.ToArray().ToMatrix()), mean);

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, varStride);
            using (mean.EnsureUnchanged())
            {
                MathLinq.Variance(data.Select(x => x.ToArray().ToMatrix()), mean, actual, ddof);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(mean);
            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 3, 3, 1)]
        [TestCase(4, 3, 5, 0)]
        public void MeanAndVariance(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expectedMean = MathNetMean(data);
            var expectedVar = MathNetVar(data, ddof);

            var result = data.Select(x => x.ToArray().ToMatrix()).MeanAndVariance(ddof);
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedVar, result.Variance, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 3, 3, 1)]
        [TestCase(4, 3, 5, 0)]
        public void MeanAndStandardDeviatione(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expectedMean = MathNetMean(data);
            var expectedStd = MathNetStd(data, ddof);

            var result = data.Select(x => x.ToArray().ToMatrix()).MeanAndStandardDeviation(ddof);
            NumAssert.AreSame(expectedMean, result.Mean, 1.0E-12);
            NumAssert.AreSame(expectedStd, result.StandardDeviation, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 3, 3, 1)]
        [TestCase(4, 3, 5, 0)]
        public void Variance_ExtensionMethod(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expected = MathNetVar(data, ddof);
            var actual = data.Select(x => x.ToArray().ToMatrix()).Variance(ddof);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 0)]
        [TestCase(1, 1, 2, 1)]
        [TestCase(2, 3, 3, 1)]
        [TestCase(4, 3, 5, 0)]
        public void StandardDeviation_ExtensionMethod(int rowCount, int colCount, int matCount, int ddof)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expected = MathNetStd(data, ddof);
            var actual = data.Select(x => x.ToArray().ToMatrix()).StandardDeviation(ddof);
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static MMat MathNetMean(IEnumerable<MMat> xs)
        {
            var first = xs.First();
            MMat sum = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(first.RowCount, first.ColumnCount);
            var count = 0;
            foreach (var x in xs)
            {
                sum += x;
                count++;
            }
            return sum / count;
        }

        private static MMat MathNetVar(IEnumerable<MMat> xs, int ddof)
        {
            var mean = MathNetMean(xs);
            
            var result = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(mean.RowCount, mean.ColumnCount);
            for (var row = 0; row < result.RowCount; row++)
            {
                for (var col = 0; col < result.ColumnCount; col++)
                {
                    if (ddof == 1)
                    {
                        result[row, col] = xs.Select(x => x[row, col]).Variance();
                    }
                    else if (ddof == 0)
                    {
                        result[row, col] = xs.Select(x => x[row, col]).PopulationVariance();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }

            return result;
        }

        private static MMat MathNetStd(IEnumerable<MMat> xs, int ddof)
        {
            var mean = MathNetMean(xs);

            var result = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(mean.RowCount, mean.ColumnCount);
            for (var row = 0; row < result.RowCount; row++)
            {
                for (var col = 0; col < result.ColumnCount; col++)
                {
                    if (ddof == 1)
                    {
                        result[row, col] = xs.Select(x => x[row, col]).StandardDeviation();
                    }
                    else if (ddof == 0)
                    {
                        result[row, col] = xs.Select(x => x[row, col]).PopulationStandardDeviation();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }

            return result;
        }

        private static MMat[] CreateData(int seed, int rowCount, int colCount, int matCount)
        {
            var random = new Random(seed);

            var data = new List<MMat>();

            for (var i = 0; i < matCount; i++)
            {
                var x = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(rowCount, colCount);
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
