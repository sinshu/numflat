using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class MatTest_Math
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.RandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => (xCol + yCol).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Mat.Add(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1.1, 1)]
        [TestCase(1, 1, 3, 4.5, 5)]
        [TestCase(2, 2, 2, 2.3, 2)]
        [TestCase(2, 2, 3, 5.5, 7)]
        [TestCase(3, 3, 3, 3.1, 3)]
        [TestCase(3, 3, 5, 4.9, 8)]
        [TestCase(1, 3, 1, 1.6, 1)]
        [TestCase(1, 3, 5, 2.0, 1)]
        [TestCase(3, 1, 3, 4.8, 3)]
        [TestCase(3, 1, 7, 7.8, 7)]
        [TestCase(2, 3, 2, 4.3, 3)]
        [TestCase(2, 3, 4, 3.3, 5)]
        [TestCase(3, 2, 3, 4.7, 6)]
        [TestCase(3, 2, 6, 4.8, 3)]
        public void Add_Scalar(int rowCount, int colCount, int xStride, double y, int dstStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (col + VectorBuilder.Fill(col.Count, y)).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Add(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.RandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => (xCol - yCol).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Mat.Sub(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1, 1.1, 1)]
        [TestCase(1, 1, 3, 4.5, 5)]
        [TestCase(2, 2, 2, 2.3, 2)]
        [TestCase(2, 2, 3, 5.5, 7)]
        [TestCase(3, 3, 3, 3.1, 3)]
        [TestCase(3, 3, 5, 4.9, 8)]
        [TestCase(1, 3, 1, 1.6, 1)]
        [TestCase(1, 3, 5, 2.0, 1)]
        [TestCase(3, 1, 3, 4.8, 3)]
        [TestCase(3, 1, 7, 7.8, 7)]
        [TestCase(2, 3, 2, 4.3, 3)]
        [TestCase(2, 3, 4, 3.3, 5)]
        [TestCase(3, 2, 3, 4.7, 6)]
        [TestCase(3, 2, 6, 4.8, 3)]
        public void Sub_Scalar(int rowCount, int colCount, int xStride, double y, int dstStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (col - VectorBuilder.Fill(col.Count, y)).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Sub(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (col * y).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Mul(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => (col / y).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Div(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.RandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => xCol.PointwiseMul(yCol).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Mat.PointwiseMul(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);
            var y = TestMatrix.NonZeroRandomDouble(57, rowCount, colCount, yStride);

            var expected = x.Cols.Zip(y.Cols, (xCol, yCol) => xCol.PointwiseDiv(yCol).AsEnumerable()).ColsToMatrix();

            var actual = TestMatrix.RandomDouble(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            using (y.EnsureUnchanged())
            {
                Mat.PointwiseDiv(x, y, actual);
            }

            NumAssert.AreSame(expected, actual, 1.0E-12);

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var actual = TestMatrix.RandomDouble(0, colCount, rowCount, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Transpose(x, actual);
            }

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    Assert.That(actual[col, row], Is.EqualTo(x[row, col]));
                }
            }

            TestMatrix.FailIfOutOfRangeWrite(actual);
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
            var x = TestMatrix.RandomDouble(42, n, n, xStride);

            var actual = x.Trace();
            var expected = Interop.ToMathNet(x).Trace();
            Assert.That(actual, Is.EqualTo(expected).Within(1.0E-12));
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 1, 3, 5)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 3, 7)]
        [TestCase(3, 3, 3, 3)]
        [TestCase(3, 3, 5, 8)]
        [TestCase(1, 3, 1, 1)]
        [TestCase(1, 3, 5, 1)]
        [TestCase(3, 1, 3, 3)]
        [TestCase(3, 1, 7, 7)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 4, 5)]
        [TestCase(3, 2, 3, 6)]
        [TestCase(3, 2, 6, 3)]
        public void Map(int rowCount, int colCount, int xStride, int dstStride)
        {
            var x = TestMatrix.RandomDouble(42, rowCount, colCount, xStride);

            var expected = x.Cols.Select(col => col.Select(value => new Complex(0, -value))).ColsToMatrix();

            Mat<Complex> actual = TestMatrix.RandomComplex(0, rowCount, colCount, dstStride);
            using (x.EnsureUnchanged())
            {
                Mat.Map(x, value => new Complex(0, -value), actual);
            }

            NumAssert.AreSame(expected, actual, 0);

            TestMatrix.FailIfOutOfRangeWrite(actual);
        }
    }
}
