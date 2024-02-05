﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class Mat_Operators_Test
    {
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 2)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 4, 3)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(3, 2, 6, 4)]
        public void Add(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.RandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => (xCol + yCol).AsEnumerable()).ColsToMatrix();

            Mat<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x + y;
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 4)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 5)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 4)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 2)]
        [TestCase(3, 1, 3, 4)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 4, 3)]
        [TestCase(3, 2, 3, 4)]
        [TestCase(3, 2, 6, 4)]
        public void Sub(int rowCount, int colCount, int xStride, int yStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.RandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => (xCol - yCol).AsEnumerable()).ColsToMatrix();

            Mat<double> actual;
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                actual = x - y;
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 2.5)]
        [TestCase(1, 1, 3, 4.1)]
        [TestCase(2, 2, 2, 2.3)]
        [TestCase(2, 2, 3, 5.4)]
        [TestCase(3, 3, 3, 3.2)]
        [TestCase(3, 3, 5, 4.0)]
        [TestCase(1, 3, 1, 1.3)]
        [TestCase(1, 3, 5, 0.2)]
        [TestCase(3, 1, 3, 0.4)]
        [TestCase(3, 1, 7, 0.7)]
        [TestCase(2, 3, 2, 4.6)]
        [TestCase(2, 3, 4, 0.3)]
        [TestCase(3, 2, 3, 4.6)]
        [TestCase(3, 2, 6, 0.4)]
        public void Mul_MatScalar(int rowCount, int colCount, int xStride, double y)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (col * y).AsEnumerable()).ColsToMatrix();
            var actual = x * y;
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 2.5)]
        [TestCase(1, 1, 3, 4.1)]
        [TestCase(2, 2, 2, 2.3)]
        [TestCase(2, 2, 3, 5.4)]
        [TestCase(3, 3, 3, 3.2)]
        [TestCase(3, 3, 5, 4.0)]
        [TestCase(1, 3, 1, 1.3)]
        [TestCase(1, 3, 5, 0.2)]
        [TestCase(3, 1, 3, 0.4)]
        [TestCase(3, 1, 7, 0.7)]
        [TestCase(2, 3, 2, 4.6)]
        [TestCase(2, 3, 4, 0.3)]
        [TestCase(3, 2, 3, 4.6)]
        [TestCase(3, 2, 6, 0.4)]
        public void Mul_ScalarMat(int rowCount, int colCount, int xStride, double y)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (y * col).AsEnumerable()).ColsToMatrix();
            var actual = y * x;
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1, 1, 1, 2.5)]
        [TestCase(1, 1, 3, 4.1)]
        [TestCase(2, 2, 2, 2.3)]
        [TestCase(2, 2, 3, 5.4)]
        [TestCase(3, 3, 3, 3.2)]
        [TestCase(3, 3, 5, 4.0)]
        [TestCase(1, 3, 1, 1.3)]
        [TestCase(1, 3, 5, 0.2)]
        [TestCase(3, 1, 3, 0.4)]
        [TestCase(3, 1, 7, 0.7)]
        [TestCase(2, 3, 2, 4.6)]
        [TestCase(2, 3, 4, 0.3)]
        [TestCase(3, 2, 3, 4.6)]
        [TestCase(3, 2, 6, 0.4)]
        public void Div(int rowCount, int colCount, int xStride, double y)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (col / y).AsEnumerable()).ColsToMatrix();
            var actual = x / y;
            NumAssert.AreSame(expected, actual, 1.0E-12);
        }
    }
}
