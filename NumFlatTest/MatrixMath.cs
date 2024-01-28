using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatrixMath
    {
        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5, 7)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4, 8)]
        [TestCase(1, 3, 1, 1, 1)]
        [TestCase(1, 3, 5, 2, 1)]
        [TestCase(3, 1, 3, 4, 3)]
        [TestCase(3, 1, 7, 7, 7)]
        [TestCase(2, 3, 2, 4, 3)]
        [TestCase(2, 3, 4, 3, 5)]
        [TestCase(3, 2, 3, 4, 6)]
        [TestCase(3, 2, 6, 4, 3)]
        public void Add(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, rowCount, colCount, dstStride);
            Mat.Add(x, y, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] + y[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5, 7)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4, 8)]
        [TestCase(1, 3, 1, 1, 1)]
        [TestCase(1, 3, 5, 2, 1)]
        [TestCase(3, 1, 3, 4, 3)]
        [TestCase(3, 1, 7, 7, 7)]
        [TestCase(2, 3, 2, 4, 3)]
        [TestCase(2, 3, 4, 3, 5)]
        [TestCase(3, 2, 3, 4, 6)]
        [TestCase(3, 2, 6, 4, 3)]
        public void Sub(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, rowCount, colCount, dstStride);
            Mat.Sub(x, y, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] - y[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5, 7)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4, 8)]
        [TestCase(1, 3, 1, 1, 1)]
        [TestCase(1, 3, 5, 2, 1)]
        [TestCase(3, 1, 3, 4, 3)]
        [TestCase(3, 1, 7, 7, 7)]
        [TestCase(2, 3, 2, 4, 3)]
        [TestCase(2, 3, 4, 3, 5)]
        [TestCase(3, 2, 3, 4, 6)]
        [TestCase(3, 2, 6, 4, 3)]
        public void PointwiseMul(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixDouble(57, rowCount, colCount, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, rowCount, colCount, dstStride);
            Mat.PointwiseMul(x, y, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] * y[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4, 5)]
        [TestCase(2, 2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5, 7)]
        [TestCase(3, 3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4, 8)]
        [TestCase(1, 3, 1, 1, 1)]
        [TestCase(1, 3, 5, 2, 1)]
        [TestCase(3, 1, 3, 4, 3)]
        [TestCase(3, 1, 7, 7, 7)]
        [TestCase(2, 3, 2, 4, 3)]
        [TestCase(2, 3, 4, 3, 5)]
        [TestCase(3, 2, 3, 4, 6)]
        [TestCase(3, 2, 6, 4, 3)]
        public void PointwiseDiv(int rowCount, int colCount, int xStride, int yStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var y = Utilities.CreateRandomMatrixNonZeroDouble(57, rowCount, colCount, yStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, rowCount, colCount, dstStride);
            Mat.PointwiseDiv(x, y, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] / y[row, col];
                    var actual = destination[row, col];
                    Assert.That(expected, Is.EqualTo(actual).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }
    }
}
