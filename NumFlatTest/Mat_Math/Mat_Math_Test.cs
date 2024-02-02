﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Math_Test
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
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
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
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 2.5, 1)]
        [TestCase(1, 1, 3, 4.1, 5)]
        [TestCase(2, 2, 2, 2.3, 2)]
        [TestCase(2, 2, 3, 5.4, 7)]
        [TestCase(3, 3, 3, 3.2, 3)]
        [TestCase(3, 3, 5, 4.0, 8)]
        [TestCase(1, 3, 1, 1.3, 1)]
        [TestCase(1, 3, 5, 0.2, 1)]
        [TestCase(3, 1, 3, 0.4, 3)]
        [TestCase(3, 1, 7, 0.7, 7)]
        [TestCase(2, 3, 2, 4.6, 3)]
        [TestCase(2, 3, 4, 0.3, 5)]
        [TestCase(3, 2, 3, 4.6, 6)]
        [TestCase(3, 2, 6, 0.4, 3)]
        public void Mul(int rowCount, int colCount, int xStride, double y, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, rowCount, colCount, dstStride);
            Mat.Mul(x, y, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] * y;
                    var actual = destination[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 2.5, 1)]
        [TestCase(1, 1, 3, 4.1, 5)]
        [TestCase(2, 2, 2, 2.3, 2)]
        [TestCase(2, 2, 3, 5.4, 7)]
        [TestCase(3, 3, 3, 3.2, 3)]
        [TestCase(3, 3, 5, 4.0, 8)]
        [TestCase(1, 3, 1, 1.3, 1)]
        [TestCase(1, 3, 5, 0.2, 1)]
        [TestCase(3, 1, 3, 0.4, 3)]
        [TestCase(3, 1, 7, 0.7, 7)]
        [TestCase(2, 3, 2, 4.6, 3)]
        [TestCase(2, 3, 4, 0.3, 5)]
        [TestCase(3, 2, 3, 4.6, 6)]
        [TestCase(3, 2, 6, 0.4, 3)]
        public void Div(int rowCount, int colCount, int xStride, double y, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, rowCount, colCount, dstStride);
            Mat.Div(x, y, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var expected = x[row, col] / y;
                    var actual = destination[row, col];
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
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
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
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
                    Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 4, 3)]
        [TestCase(3, 1, 3, 1)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(1, 3, 1, 3)]
        [TestCase(1, 3, 2, 5)]
        [TestCase(3, 2, 3, 2)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(4, 5, 8, 7)]
        [TestCase(9, 6, 11, 7)]
        public void Transpose(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, rowCount, colCount, xStride);
            var destination = Utilities.CreateRandomMatrixDouble(0, colCount, rowCount, dstStride);
            Mat.Transpose(x, destination);

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(x[row, col] == destination[col, row]);
                }
            }

            Utilities.FailIfOutOfRangeWrite(destination);
        }

        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        [TestCase(3, 5)]
        [TestCase(10, 11)]
        public void Trace(int n, int xStride)
        {
            var x = Utilities.CreateRandomMatrixDouble(42, n, n, xStride);
            var actual = x.Trace();

            var expected = Utilities.ToMathNet(x).Trace();

            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }
    }
}
