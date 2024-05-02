using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using NumFlat;

using MVec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using MMat = MathNet.Numerics.LinearAlgebra.Matrix<double>;

namespace NumFlatTest.MathLinqTests
{
    public class MatrixSumTestsDouble
    {
        [TestCase(1, 1, 1, 2)]
        [TestCase(1, 1, 3, 2)]
        [TestCase(3, 2, 1, 4)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(4, 5, 6, 5)]
        [TestCase(3, 6, 4, 4)]
        public void Sum(int rowCount, int colCount, int matCount, int dstStride)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expected = MathNetSum(data);

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            MathLinq.Sum(data.Select(x => x.ToArray().ToMatrix()), actual);

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 1, 3)]
        [TestCase(3, 2, 1)]
        [TestCase(3, 2, 3)]
        [TestCase(4, 5, 6)]
        [TestCase(3, 6, 4)]
        public void Sum_ExtensionMethod(int rowCount, int colCount, int matCount)
        {
            var data = CreateData(42, rowCount, colCount, matCount);
            var expected = MathNetSum(data);
            var actual = data.Select(x => x.ToArray().ToMatrix()).Sum();
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        private static MMat MathNetSum(IEnumerable<MMat> xs)
        {
            var first = xs.First();
            MMat sum = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(first.RowCount, first.ColumnCount);
            foreach (var x in xs)
            {
                sum += x;
            }
            return sum;
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
